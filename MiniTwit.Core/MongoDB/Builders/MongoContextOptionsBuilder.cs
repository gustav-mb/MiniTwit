using System.Reflection;
using MongoDB.Driver;

namespace MiniTwit.Core.MongoDB.Builders;

public class MongoContextOptionsBuilder : IMongoContextOptionsBuilder
{
    private readonly IDictionary<Type, object> _entityToBuilder = new Dictionary<Type, object>();
    private bool _isConfigured;
    public IMongoDatabase Database { get; }

    public MongoContextOptionsBuilder(IMongoDatabase database)
    {
        Database = database;
    }

    public void Configure(MongoContext context, Action<IMongoContextOptionsBuilder> onConfiguring)
    {
        if (!_isConfigured)
        {
            onConfiguring.Invoke(this);
        }

        // Get IMongoCollection properties from MongoContext
        var contextProperties = context.GetType().GetRuntimeProperties()
        .Where(
            p => !(p.GetMethod ?? p.SetMethod)!.IsStatic
                && !p.GetIndexParameters().Any()
                && p.DeclaringType != typeof(MongoContext)
                && p.PropertyType.GetTypeInfo().IsGenericType
                && p.PropertyType.GetGenericTypeDefinition() == typeof(IMongoCollection<>))
        .OrderBy(p => p.Name)
        .Select(p => (p.Name, Type: p.PropertyType.GenericTypeArguments.Single()))
        .ToArray();

        // Set value of each collection property
        foreach (var property in contextProperties)
        {
            var mongoCollectionType = typeof(IMongoCollection<>).MakeGenericType(property.Type);
            var mongoCollection = GetMongoCollectionInstance(property);

            context.GetType().GetProperty(property.Name)!.SetValue(context, mongoCollection);
        }

         ConfigureIndexes();

        _isConfigured = true;
    }

    private object GetMongoCollectionInstance((string Name, Type Type) property)
    {
        // Create EntityBuilder of type Type
        var builderType = typeof(EntityTypeBuilder<>).MakeGenericType(property.Type);
        
        // Get existing EntityBuilder or create one if it doesn't exist
        var entityBuilder = _entityToBuilder.ContainsKey(property.Type) ? _entityToBuilder[property.Type] : Activator.CreateInstance(builderType, Database)!;
        
        // Get Collection property of EntityBuilder
        var collectionProperty = builderType.GetProperty("Collection")!;
        var mongoCollection = collectionProperty.GetValue(entityBuilder);

        // MongoCollection is not initialized
        if (mongoCollection == null)
        {
            MethodInfo getCollectionMethod = Database.GetType().GetMethod("GetCollection") ?? throw new InvalidOperationException("IMongoDatabase does not expose GetCollection method");
            MethodInfo genericGetCollectionMethod = getCollectionMethod.MakeGenericMethod(property.Type);

            // Get new collection from MongoDB database
            mongoCollection = genericGetCollectionMethod.Invoke(Database, new object?[] { property.Name, null }) ?? throw new InvalidOperationException($"GetCollection<{property.Type}>(\"{property.Name}\" does not return a value)");

            // Set Collection property of Builder
            collectionProperty.SetValue(entityBuilder, mongoCollection);

            // Add Builder to map
            _entityToBuilder[property.Type] = entityBuilder;
        }

        return mongoCollection;
    }

    public EntityTypeBuilder<TEntity> Entity<TEntity>(Action<EntityTypeBuilder<TEntity>> buildAction) where TEntity : class
    {
        var builder = _entityToBuilder.ContainsKey(typeof(TEntity)) ? _entityToBuilder[typeof(TEntity)] as EntityTypeBuilder<TEntity> : new EntityTypeBuilder<TEntity>(Database);
        buildAction.Invoke(builder!);
        _entityToBuilder[typeof(TEntity)] = builder!;
        return builder!;
    }

    private void ConfigureIndexes()
    {
        foreach (var pair in _entityToBuilder)
        {
            var builderType = typeof(EntityTypeBuilder<>).MakeGenericType(pair.Key);
            var builder = _entityToBuilder[pair.Key];

            // Call ConfigureIndexes on builder
            builderType.InvokeMember("ConfigureIndexes", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic, Type.DefaultBinder, builder, null);
        }
    }
}