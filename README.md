Обмен начинается автоматически при запуске системы.

Commands:
-cleanup
-start

CleanUp 
1. Запуск модуля останавливает работу основной системы. 
2. Система перебирает все ОК кошельки, которые в данный момент активны и совершает обмен всех имеющихся на кошельках токенов на USDC (исключение: SOL-токены, их Система не трогает) 3. После выполнения п.2, все имеющиеся USDC-токены на балансе ОК Система переводит на ГК. 
4. После выполнения п.3, все имеющиеся SOL-токены на балансе ОК Система переводит на ГК.

Start
Обновляет баланс кошельков. Запускает систему в течении 5 минут.

Конфигурация системы находится в файле 'local.settings.json'

Пример:
{
  "ConnectionStrings": {
    "SqlConnectionString": "Server = db; DataBase = mmTransaction; Integrated Security = false; User Id = postgres; password = 4241; Port=5432;",
    "CryptoApiUrl": "http://134.209.216.186:1832/api/crypto/",
    "JupApiUrl": "https://quote-api.jup.ag/v3/",
    "RaydiumApiUrl": "https://api.raydium.io/v2/"
  },
  "Settings": {
    //Горячий кошелёк
    "HotWallet": {
      "PublicKey": "5Bma4BvxiqmW5szfKmGb8H3rrGTdu659ZKtZbs4PMD3j",
      "PrivateKey": "z/QoSzx0Ifz5rvFuwLN4r9wkfnEfn28FzMUq/pCCVCfrwgHzzCJfkbgK0iAtXCqhmgy0h5hxbhfetXT26g1pmcK1y6yDSfqqy5D2EjKBWc5h1v8FiiI/K9plZxM4GAXom4WzRbVmIQ=="
    },
    //Операционные кошельки
    "ColdWallet": [
      {
        "PublicKey": "AFs8VxTyjgudKXN2LPPK27L6SQTVzwByRDoEyLm6k5wM",
        "PrivateKey": "0D2qWgnnqL9OhekmgTANJ1pbFHwiYn/QC+rbQsz7hxtLgrRb11cGYzyjoMI23N9GieKWNILXhuBYQCelgL24ebqcmofn1xBJTpRKm9O0CiZ+xOLcD7lR6sl7kJfXduFYol9NvqebrA=="
      },
      {
        "PublicKey": "HFukhgfUF4nWXGipH8kaiKAstWfMgVELnkXQuxFjVTZ1",
        "PrivateKey": "XRi/MvWCtQkB6iH7+odzT0ZiQma4iE/33BIVQFub8vZ18HrxIaWrPECWYDQ+yC5+3ZDp47p8hw1vRhVbb/q1V3fkJlmy108I2plB3qbl2g7TfIlWFEQmWV3DGlN7nLdqqiDbYSLY9Q=="
      }
    ],
    "BaseVolumeSOLperColdWallet": 0.001, // Базовый объем SOL для ОК-кошелька
    "BaseVolumeUSDCperColdWallet": 0.1, // Базовый объем USDC для ОК-кошелька
    "DailyTradingVolumeInUSDCperXtoken": 2, // Суточный объем торгов в USDC для X-token
    "PercentageOfRandomTransactionsForAddToken": 0.05, // Процент случайных транзакций для ADD-token
    "MinimumDelayInSecondsForOneTransactionPerWallet": 180, // Минимальная задержка (в мин) между двумя операциями для одного ОК-кошелек
    "AutomaticGenerationOfWallets": false, // Автоматическая генерация кошельков, в ином случае импортировать из конфигурации
    "ColdWalletsCount": 10, // Количество генерируемых ОК кошельков
    "USDCmint": "EPjFWdd5AufqSSqeM2qN1xzybapC8G4wEGGkZwyTDt1v", // Токен USDC
    "XTokenMint": "So11111111111111111111111111111111111111112" // Основной токен для обменов (X-token)
  }
}
