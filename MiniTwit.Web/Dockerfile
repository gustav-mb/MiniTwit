FROM node:lts-alpine

ENV NODE_ENV=production

WORKDIR /usr/src/app

COPY ["MiniTwit.Web/package.json", "MiniTwit.Web/package-lock.json*", "./"]
RUN npm install --production --silent && mv node_modules ../

COPY "MiniTwit.Web/." .
RUN if [ "$NODE_ENV" = "production" ] ; then npm run build && npm install -g serve ; fi

EXPOSE 3000

RUN chown -R node /usr/src/app
USER node

RUN echo "if [ "\$NODE_ENV" = 'production' ] ; then serve -s build ; else npm run start; fi" > run.sh
RUN chmod 777 ./run.sh

CMD ./run.sh