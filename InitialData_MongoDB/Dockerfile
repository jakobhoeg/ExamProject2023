FROM mongo:7

COPY ./babynames_list.json /babynames_list.json
COPY ./users.json /users.json

CMD bash -c 'mongoimport --host mongohost --db mongodb --collection BabyName --type json --file /babynames_list.json --jsonArray && mongoimport --host mongohost --db mongodb --collection User --type json --file /users.json --jsonArray'