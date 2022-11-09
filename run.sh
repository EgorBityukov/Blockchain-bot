docker container stop mm-bot
docker container rm mm-bot
docker pull flerry/mm-bot:latest
docker run -itd -p 25060:25060 --name mm-bot flerry/mm-bot:latest


