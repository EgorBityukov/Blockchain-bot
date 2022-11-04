<h3> ����� ���������� ������������� ��� ������� �������. </h3>

<div>
<b>Commands:</b><br>
-cleanup<br>
-start<br>
</div>
<br>
<div>
<h3>CleanUp</h3> <br>
1. ������ ������ ������������� ������ �������� �������. <br>
2. ������� ���������� ��� �� ��������, ������� � ������ ������ ������� � ��������� ����� ���� ��������� �� ��������� ������� �� USDC (����������: SOL-������, �� ������� �� �������) 3. ����� ���������� �.2, ��� ��������� USDC-������ �� ������� �� ������� ��������� �� ��. <br>
4. ����� ���������� �.3, ��� ��������� SOL-������ �� ������� �� ������� ��������� �� ��.<br>
</div>
<br>
<div>
<h3>Start</h3><br>
��������� ������ ���������. ��������� ������� � ������� 5 �����.<br>
</div>
<br>
<div>
������������ ������� ��������� � ����� 'local.settings.json'
</div>
<br>
<div>
<h3>�������� �������:</h3><br>
<br>
{<br>
  "ConnectionStrings": {<br>
    "SqlConnectionString": "Server = db; DataBase = mmTransaction; Integrated Security = false; User Id = postgres; password = 4241; Port=5432;",<br>
    "CryptoApiUrl": "http://134.209.216.186:1832/api/crypto/",<br>
    "JupApiUrl": "https://quote-api.jup.ag/v3/",<br>
    "RaydiumApiUrl": "https://api.raydium.io/v2/"<br>
  },<br>
  "Settings": {<br>
    //������� ������<br>
    "HotWallet": {<br>
      "PublicKey": "5Bma4BvxiqmW5szfKmGb8H3rrGTdu659ZKtZbs4PMD3j",<br>
      "PrivateKey": "z/QoSzx0Ifz5rvFuwLN4r9wkfnEfn28FzMUq/pCCVCfrwgHzzCJfkbgK0iAtXCqhmgy0h5hxbhfetXT26g1pmcK1y6yDSfqqy5D2EjKBWc5h1v8FiiI/K9plZxM4GAXom4WzRbVmIQ=="<br>
    },<br>
    //������������ ��������<br>
    "ColdWallet": [<br>
      {<br>
        "PublicKey": "AFs8VxTyjgudKXN2LPPK27L6SQTVzwByRDoEyLm6k5wM",<br>
        "PrivateKey": "0D2qWgnnqL9OhekmgTANJ1pbFHwiYn/QC+rbQsz7hxtLgrRb11cGYzyjoMI23N9GieKWNILXhuBYQCelgL24ebqcmofn1xBJTpRKm9O0CiZ+xOLcD7lR6sl7kJfXduFYol9NvqebrA=="<br>
      },<br>
      {<br>
        "PublicKey": "HFukhgfUF4nWXGipH8kaiKAstWfMgVELnkXQuxFjVTZ1",<br>
        "PrivateKey": "XRi/MvWCtQkB6iH7+odzT0ZiQma4iE/33BIVQFub8vZ18HrxIaWrPECWYDQ+yC5+3ZDp47p8hw1vRhVbb/q1V3fkJlmy108I2plB3qbl2g7TfIlWFEQmWV3DGlN7nLdqqiDbYSLY9Q=="<br>
      }<br>
    ],<br>
    "BaseVolumeSOLperColdWallet": 0.001, // ������� ����� SOL ��� ��-��������<br>
    "BaseVolumeUSDCperColdWallet": 0.1, // ������� ����� USDC ��� ��-��������<br>
    "DailyTradingVolumeInUSDCperXtoken": 2, // �������� ����� ������ � USDC ��� X-token<br>
    "PercentageOfRandomTransactionsForAddToken": 0.05, // ������� ��������� ���������� ��� ADD-token<br>
    "MinimumDelayInSecondsForOneTransactionPerWallet": 180, // ����������� �������� (� ���) ����� ����� ���������� ��� ������ ��-�������<br>
    "AutomaticGenerationOfWallets": false, // �������������� ��������� ���������, � ���� ������ ������������� �� ������������<br>
    "ColdWalletsCount": 10, // ���������� ������������ �� ���������<br>
    "USDCmint": "EPjFWdd5AufqSSqeM2qN1xzybapC8G4wEGGkZwyTDt1v", // ����� USDC<br>
    "XTokenMint": "So11111111111111111111111111111111111111112" // �������� ����� ��� ������� (X-token)<br>
  }<br>
}<br>
</div>