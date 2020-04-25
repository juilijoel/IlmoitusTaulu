# IlmoitusTaulu
RSS Feed -> Telegram integration

Fetches posts from RSS feed and posts them to a Telegram channel. Uses Azure Cosmos DB to store timestamp value of latest post.
Should be used as a triggered scheduled Webjob or triggered by Task Scheduler.

# How to deploy
1. Fill in needed settings in appsettings_example.json and rename it to appsettings.json
2. Run or schedule application
