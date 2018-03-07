App = {
  web3Provider: null,
  contracts: {},

  init: function() {
    // Load pets.
   
    return App.initWeb3();
  },

  initWeb3: function() {
	    if (typeof web3 !== 'undefined') {
		  App.web3Provider = web3.currentProvider;
		} else {
		  // If no injected web3 instance is detected, fall back to Ganache
		  App.web3Provider = new Web3.providers.HttpProvider('http://localhost:7545');
		}
		web3 = new Web3(App.web3Provider);

	    return App.initContract();
  	},

  initContract: function() {
   $.getJSON('../truffle/build/contracts/MetaCoin.json', function(data) {
  // Get the necessary contract artifact file and instantiate it with truffle-contract
	  var MetaCoinArtifact = data;
	  App.contracts.MetaCoin = TruffleContract(MetaCoinArtifact);

	  // Set the provider for our contract
	  App.contracts.MetaCoin.setProvider(App.web3Provider);
	});
   	
  },
  sendCoin: function(senderIndex, receiverIndex, amt) {
	    var metaCoinInstance;

		App.contracts.MetaCoin.deployed()
		.then(function(instance) {
		  metaCoinInstance = instance;
		  metaCoinInstance.sendCoin(web3.eth.accounts[receiverIndex], amt, {from: web3.eth.accounts[senderIndex]})
		})
		.then(function(results) {
		  return metaCoinInstance.getBalance.call(web3.eth.accounts[receiverIndex]);
		})
		.then(function(metaCoin) {
			console.log(metaCoin.toNumber());
			return metaCoinInstance.getBalance.call(web3.eth.accounts[senderIndex]);	
		})
		.then(function(senderAccount) {
			console.log(senderAccount.toNumber());
		})
		.catch(function(err) {
			  console.log(err.message);
		});
	},
	getBalance: function(accountIndex) {
		var coinInstance;
		App.contracts.MetaCoin.deployed()
		.then(function(instance) {
			coinInstance = instance;
			return coinInstance.getBalance(web3.eth.accounts[accountIndex])
		})
		.then(function(balance) {
			console.log(balance.toNumber());
		})
		.catch(function(err) {
			console.log(err)
		});
	},
	buyCoins: function(buyerIndex, amt) {
		var coinInstance;
		App.contracts.MetaCoin.deployed()
		.then(function(instance) {
			coinInstance = instance;
			return coinInstance.sendTransaction({from: web3.eth.accounts[buyerIndex], value: web3.toWei(amt, "ether")})
		})
		.then(function(buyResults) {
			console.log(buyResults);
		})
		.catch(function(err) {
			console.log(err)
		});

	},
	createCoins: function(recipient, amt) {
		var coinInstance;
		App.contracts.MetaCoin.deployed()
		.then(function(instance) {
			coinInstance = instance;
			return coinInstance.createCoin(web3.eth.accounts[recipient], amt);
		})
		.then(function(deositResults) {
			console.log(buyResults);
		})
		.catch(function(err) {
			console.log(err)
		});
	},
}
$(function() {

    App.init();

});
