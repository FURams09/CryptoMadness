MarchMadnessApp = {
    web3Provider: null,
	contracts: {},
	poolFee: 0,
	entryFee: 0,
	salesCommission: 0,

    init: function() {
     
      return MarchMadnessApp.initWeb3();
    },
  
    initWeb3: function() {
        if (typeof web3 !== 'undefined') {
        MarchMadnessApp.web3Provider = web3.currentProvider;
        } else {
        // If no injected web3 instance is detected, fall back to Ganache
        var webProviderAddress = 'http://localhost:7545';
        console.log('fallback URL in use. MetaMask not injected. local address: ' + webProviderAddress, )
		MarchMadnessApp.web3Provider = new Web3.providers.HttpProvider(webProviderAddress);
        }
		web3 = new Web3(MarchMadnessApp.web3Provider);
  
        return MarchMadnessApp.initContract();
    },
  
    initContract: function() {
		$.getJSON('/Contracts/build/contracts/MarchMadness.json', function (data) {
			// Get the necessary contract artifact file and instantiate it with truffle-contract
			var MarchMadnessArtifact = data;
			MarchMadnessApp.contracts.MarchMadness = TruffleContract(MarchMadnessArtifact);

			// Set the provider for our contract
			MarchMadnessApp.contracts.MarchMadness.setProvider(MarchMadnessApp.web3Provider);
			MarchMadnessApp.setInstaceVariables()

		});

         
	},
    hashBracket: function (bracket) {
        return web3.sha3(bracket.toString());
	},
	setInstaceVariables: async function () {
		var march_madness;
		MarchMadnessApp.contracts.MarchMadness.deployed()
			.then(function (instance) {
				march_madness = instance;
				console.log(march_madness);
				return march_madness.PoolFee.call({ from: web3.eth.accounts[0] })
			})
			.then((poolFee) => {
				MarchMadnessApp.poolFee = poolFee.toNumber();
				return march_madness.EntryFee.call({ from: web3.eth.accounts[0] })
			})
			.then((entryFee) => {
				MarchMadnessApp.entryFee = entryFee.toNumber();
				return march_madness.SalesCommission.call({ from: web3.eth.accounts[0] })
			})
			.then((salesCommission) => {
				MarchMadnessApp.SalesCommission = salesCommission.toNumber();
			});
	},
	
    ContractCalls: {
		createPool: async function (poolId, poolEntryFee, senderAddress) {
			var march_madness = await MarchMadnessApp.contracts.MarchMadness.deployed();
			var createPoolWatch = march_madness.PoolCreated()
			var createPoolErrorWatch = march_madness.PaymentError()
			march_madness.createPool(poolId, web3.toWei(poolEntryFee, "ether"), { from: senderAddress, gas: 1000000, value: MarchMadnessApp.poolFee });

			createPoolWatch.watch(function (err, res) {
				if (!err) {
					console.log(res);
				}
				console.log(err);
			})
			createPoolErrorWatch.watch(function (err, res) {
				if (!err) {
					console.log(res);
				}
				console.log(err);
			})

            
        },
        getPoolBalance: async function(poolId) {
			var march_madness = await MarchMadnessApp.contracts.MarchMadness.deployed();
			return march_madness.getPoolValue.call(poolId, { from: web3.eth.accounts[0] });
		},
		getPoolEntryFee: async function (poolId) {
			var march_madness = await MarchMadnessApp.contracts.MarchMadness.deployed();
			return march_madness.getPoolEntryFee(poolId, {from: web3.eth.accounts[0]})
		},
		createBracket: async function (bracket, poolId, senderAddress) {
			var march_madness = await MarchMadnessApp.contracts.MarchMadness.deployed();
			var poolEntryFee = await march_madness.getPoolEntryFee(poolId, { from: senderAddress }).then((res) => { return res.toNumber(); });
			var storableBracket = MarchMadnessApp.hashBracket(bracket)
			var createBracketWatch = march_madness.BracketCreated();
			march_madness.createBracket(storableBracket, poolId, { from: senderAddress, value: parseInt(MarchMadnessApp.entryFee) + parseInt(poolEntryFee), gas: 120000 })
			createBracketWatch.watch(function (err, res) {
				if (!err) {
					console.log(res);
				} else {
					console.log(err);
				}
			})
        },
        payWinner: async function(winner, poolId, bracket, senderAddress) {
			var march_madness = await MarchMadnessApp.contracts.MarchMadness.deployed();
			var storableBracket = MarchMadnessApp.hashBracket(bracket);
			var payWinnerWatch = march_madness.PaymentMade();
			var payErrorWatch = march_madness.PaymentError();
			march_madness.payWinner(winner, poolId, storableBracket, { from: senderAddress, gas: 100000 })
				.catch(function (err) {
					console.log(err);
				})

			payWinnerWatch.watch(function (err, res) {
				if (!err) {
					console.log(res);
				} else {
					console.log(err);
				}
			});
			payErrorWatch.watch(function (err, res) {
				if (!err) {
					console.log(res);
				} else {
					console.log(err);
				}
			})
          
        },
        timeLeftUntilLock: function(sender) {
            var MM;
            MarchMadnessApp.contracts.MarchMadness.deployed()
            .then(function(instance) {
                MM = instance;
                return MM.secondsUntilLock.call({sender: sender, gasPrice: MMStore.gasPrice, gas: MMStore.maxGas})
            })
            .then(function(secondsLeft) {

                console.log(secondsLeft.toNumber(), 'seconds left til lock');
       
            })
            .catch(function(err) {
                console.log(err)
            });
        }
    }, 
    Admin: {
        withdraw:  async function(amount, recipient, approvingAdmin) {
            var march_madness = await MarchMadnessApp.contracts.MarchMadness.deployed()
			var paymentWatch = march_madness.PaymentMade();
			return march_madness.withdrawFunds(recipient, amount, {from: approvingAdmin})

				paymentWatch
            .catch(function(err) {
                console.log(err)
            });

        },
        getContractInfo: function(sender) {
            var MM;
            MarchMadnessApp.contracts.MarchMadness.deployed()
            .then(function(instance) {
                MM = instance;
                return MM.getContractBalance.call({from: sender})
            })
            .then(function(balances) {
				if (balances) {
                    console.log(balances);
                } else {
                    console.log('Bracket Update Completed but not saved');
                };
            })
            .catch(function(err) {
                console.log(err)
            });

        },
        addAdmin: function(grantor, newAdmin) {
            var MM;
            MarchMadnessApp.contracts.MarchMadness.deployed()
            .then(function(instance) {
                MM = instance;
                return MM.addAdmin(newAdmin)
            })
            .then(function(success) {
                if(success) {
                    console.log(bracketAdded);
                } else {
                    console.log('Bracket Update Completed but not saved');
                };
            })
            .catch(function(err) {
                console.log(err)
            });
        },
        removeAdmin: function(judger, disgracedAdmin) {
            var MM;
            MarchMadnessApp.contracts.MarchMadness.deployed()
            .then(function(instance) {
                MM = instance;
                return MM.removeAdmin(disgracedAdmin, {from: judger, gasPrice: MMStore.gasPrice, gas: MMStore.maxGas})
            })
            .then(function(success) {
                console.log(adminRemoved);
            })
            .catch(function(err) {
                console.log(err)
            });
        },
        updateFee: function(admin, feeType, newFee) {
            var MM;
            MarchMadnessApp.contracts.MarchMadness.deployed()
            .then(function(instance) {
                MM = instance;
                switch (feeType) {
                    case "Pool": 
                        return MM.updatePoolFee(admin, {from: admin, gasPrice: MMStore.gasPrice, gas: MMStore.maxGas})
                    case "Entry":
                        return MM.updateEntryFee(admin, {from: admin, gasPrice: MMStore.gasPrice, gas: MMStore.maxGas})
                    case "SalesCommission":
                        return MM.updateSalesCommission(admin, {from: admin, gasPrice: MMStore.gasPrice, gas: MMStore.maxGas})
                    default:
                        throw('Incorrect feeType: ' + feeType);
                }
                
            })
            .then(function(success) {
                console.log(adminRemoved);
            })
            .catch(function(err) {
                console.log(err)
            });
        },
        
    }


}


  $(function() {
  
      MarchMadnessApp.init();
  
  });
  