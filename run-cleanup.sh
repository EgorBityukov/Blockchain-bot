docker container stop mm-bot
docker container rm mm-bot
docker run -itd -p 25060:25060 --name mm-bot flerry/mm-bot:latest cleanup