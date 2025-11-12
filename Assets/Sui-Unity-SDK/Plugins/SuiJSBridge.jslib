mergeInto(LibraryManager.library, 
{

  OpenURL : function(url)
  {
	url = UTF8ToString(url);
	window.open(url,'_blank');
  },
  
  GoogleLogin : function(clientId, nonce)
  {
	nonce = UTF8ToString(nonce);
	clientId = UTF8ToString(clientId);
	window.googleLogin(clientId, nonce);
  },
  
  StartEncrypt : function(data, packageId, suiAddress, nonceB64, suiClientUrl, serverObjectIds, threshold)
  {
	data = UTF8ToString(data);
	packageId = UTF8ToString(packageId);
	suiAddress = UTF8ToString(suiAddress);
	nonceB64 = UTF8ToString(nonceB64);
	suiClientUrl = UTF8ToString(suiClientUrl);
	var jsonString = UTF8ToString(serverObjectIds);
	var serverObjectIdsData = JSON.parse(jsonString);
	window.sealEncrypt(data, packageId, suiAddress, nonceB64, suiClientUrl, serverObjectIdsData.items, threshold);
  },
  
  StartDecrypt : function(encryptedBytesBase64, txBytesBase64, privateKeyB64, suiAddress, packageId, suiClientUrl, serverObjectIds)
  {
	encryptedBytesBase64 = UTF8ToString(encryptedBytesBase64);
	txBytesBase64 = UTF8ToString(txBytesBase64);
	privateKeyB64 = UTF8ToString(privateKeyB64);
	suiAddress = UTF8ToString(suiAddress);
	packageId = UTF8ToString(packageId);
	suiClientUrl = UTF8ToString(suiClientUrl);
	var jsonString = UTF8ToString(serverObjectIds);
	var serverObjectIdsData = JSON.parse(jsonString);
	window.sealDecrypt(encryptedBytesBase64, txBytesBase64, privateKeyB64, suiAddress, packageId, suiClientUrl, serverObjectIdsData.items);
  },
  
  StartDecryptWithZKLogin: function(encryptedBytesBase64, txBytesBase64, ephemeralPrivateKeyB64, inputBytesB64, maxEpoch, suiAddress, packageId, suiClientUrl, serverObjectIds)
  {
	encryptedBytesBase64 = UTF8ToString(encryptedBytesBase64);
	txBytesBase64 = UTF8ToString(txBytesBase64);
	ephemeralPrivateKeyB64 = UTF8ToString(ephemeralPrivateKeyB64);
	inputBytesB64 = UTF8ToString(inputBytesB64);
	suiAddress = UTF8ToString(suiAddress);
	packageId = UTF8ToString(packageId);
	suiClientUrl = UTF8ToString(suiClientUrl);
	var jsonString = UTF8ToString(serverObjectIds);
	var serverObjectIdsData = JSON.parse(jsonString);
	window.sealDecryptWithZKLogin(encryptedBytesBase64, txBytesBase64, ephemeralPrivateKeyB64, inputBytesB64, maxEpoch, suiAddress, packageId, suiClientUrl, serverObjectIdsData.items);
  }


});