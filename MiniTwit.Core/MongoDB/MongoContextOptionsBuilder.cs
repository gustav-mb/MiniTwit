using System.Reflection;
using MongoDB.Driver;

namespace MiniTwit.Core.MongoDB;

public class MongoContextOptionsBuilder : IMongoContextOptionsBuilder
{
    private bool _isConfigured;
    public IMongoDatabase Database { get; }

    public MongoContextOptionsBuilder(IMongoDatabase database)
    {
        Database = database;
    }

    public void Configure(MongoContext context, Action onConfiguring)
    {
        if (!_isConfigured)
        {
            onConfiguring.Invoke();
        }

        // Get MongoCollection properties in context
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

        _isConfigured = true;
    }

    private object GetMongoCollectionInstance((string Name, Type Type) property)
    {
        MethodInfo getCollectionMethod = Database.GetType().GetMethod("GetCollection") ?? throw new InvalidOperationException("IMongoDatase does not expose GetCollection method");
        MethodInfo genericGetCollectionMethod = getCollectionMethod.MakeGenericMethod(property.Type);

        var mongocollection = genericGetCollectionMethod.Invoke(Database, new object?[] { property.Name, null }) ?? throw new InvalidOperationException($"GetCollection<{property.Type}>(\"{property.Name}\" does not return a value)");

        return mongocollection;
    }
}