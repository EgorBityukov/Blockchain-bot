����� ���������� ������������� ��� ������� �������.

Commands:
-cleanup
-start

CleanUp 
1. ������ ������ ������������� ������ �������� �������. 
2. ������� ���������� ��� �� ��������, ������� � ������ ������ ������� � ��������� ����� ���� ��������� �� ��������� ������� �� USDC (����������: SOL-������, �� ������� �� �������) 3. ����� ���������� �.2, ��� ��������� USDC-������ �� ������� �� ������� ��������� �� ��. 
4. ����� ���������� �.3, ��� ��������� SOL-������ �� ������� �� ������� ��������� �� ��.

Start
��������� ������ ���������. ��������� ������� � ������� 5 �����.

������������ ������� ��������� � ����� 'local.settings.json'

������:
{
  "ConnectionStrings": {
    "SqlConnectionString": "Server = db; DataBase = mmTransaction; Integrated Security = false; User Id = postgres; password = 4241; Port=5432;",
    "CryptoApiUrl": "http://134.209.216.186:1832/api/crypto/",
    "JupApiUrl": "https://quote-api.jup.ag/v3/",
    "RaydiumApiUrl": "https://api.raydium.io/v2/"
  },
  "Settings": {
    //������� ������
    "HotWallet": {
      "PublicKey": "5Bma4BvxiqmW5szfKmGb8H3rrGTdu659ZKtZbs4PMD3j",
      "PrivateKey": "z/QoSzx0Ifz5rvFuwLN4r9wkfnEfn28FzMUq/pCCVCfrwgHzzCJfkbgK0iAtXCqhmgy0h5hxbhfetXT26g1pmcK1y6yDSfqqy5D2EjKBWc5h1v8FiiI/K9plZxM4GAXom4WzRbVmIQ=="
    },
    //������������ ��������
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
    "BaseVolumeSOLperColdWallet": 0.001, // ������� ����� SOL ��� ��-��������
    "BaseVolumeUSDCperColdWallet": 0.1, // ������� ����� USDC ��� ��-��������
    "DailyTradingVolumeInUSDCperXtoken": 2, // �������� ����� ������ � USDC ��� X-token
    "PercentageOfRandomTransactionsForAddToken": 0.05, // ������� ��������� ���������� ��� ADD-token
    "MinimumDelayInSecondsForOneTransactionPerWallet": 180, // ����������� �������� (� ���) ����� ����� ���������� ��� ������ ��-�������
    "AutomaticGenerationOfWallets": false, // �������������� ��������� ���������, � ���� ������ ������������� �� ������������
    "ColdWalletsCount": 10, // ���������� ������������ �� ���������
    "USDCmint": "EPjFWdd5AufqSSqeM2qN1xzybapC8G4wEGGkZwyTDt1v", // ����� USDC
    "XTokenMint": "So11111111111111111111111111111111111111112" // �������� ����� ��� ������� (X-token)
  }
}