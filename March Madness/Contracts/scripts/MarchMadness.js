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
        var webProviderAddress = 'http://localhost:9545';
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
	setInstaceVariables:  function () {
		var march_madness = await MarchMadnessApp.contracts.MarchMadness.deployed(async function () {
			MarchMadnessApp.poolFee = await march_madness.PoolFee.call({ from: web3.eth.accounts[0] }).then((poolFee) => { return poolFee.toNumber(); });
			MarchMadnessApp.entryFee = await march_madness.EntryFee.call({ from: web3.eth.accounts[0] }).then((entryFee) => { return entryFee.toNumber(); });
		})
	},
	
    ContractCalls: {
		createPool: async function (sender, poolId, poolEntryFee) {
			var march_madness = await MarchMadnessApp.contracts.MarchMadness.deployed();
			//only for testing otherwise all pools will be poolId
			poolId = poolId || 1
			poolEntryFee = poolEntryFee || 0.025
			await march_madness.createPool(poolId, poolEntryFee, { from: sender, gas: 1000000})
			//start watch. does nothing yet
			MarchMadnessApp.march_madness.createPool.watch(function (error, result) {
				if (!error) {
					console.log(result);
				}
				else {
					console.log(error);
				}
			});
			
            
        },
        getPoolBalance: function(poolId, caller) {
            var MM;
            MarchMadnessApp.contracts.MarchMadness.deployed()
            .then(function(instance) {
                MM = instance;
                return MM.getPoolValueInEth(poolId, {from: caller})
            })
            .then(function(poolValue) {
                console.log(poolValue.toNumber());
            })
            .catch(function(err) {
                console.log(err)
            });
        },
        createBracket: function(bracket, poolId, entrant) {
			var MM;
			var storableBracket = MarchMadnessApp.hashBracket(bracket)
            MarchMadnessApp.contracts.MarchMadness.deployed()
            .then(function(instance) {
                MM = instance;
				return MM.createBracket(storableBracket, MMStore.poolId, { from: entrant, value: parseInt(MMStore.entryFee) + parseInt(MMStore.poolEntryFee), gas: 50000  })
            })
            .then(function(success) {
                if(success) {
					console.log( success);
                } else {
                console.log('Bracket Creation Completed but not saved');
                };
            })
            .catch(function(err) {
                console.log(err)
            });

        },
        updateBracket: function(bracket, entrant) {
			var MM;
			var storableBracket = MarchMadnessApp.hashBracket(bracket);
            MarchMadnessApp.contracts.MarchMadness.deployed()
            .then(function(instance) {
                MM = instance;
				return MM.updateBracket(storableBracket, { from: entrant })
            })
            .then(function(success) {
                if(success) {
                    console.log(storableBracket, success);
                } else {
                    console.log('Bracket Update Completed but not saved');
                };
            })
            .catch(function(err) {
                console.log(err)
            });

        },
        claimWinnings: function(winner, poolId, bracket) {
            //call this to get your winnings but the winner pays the gas. otherwise wait for payment from Close
            var MM;
            MarchMadnessApp.contracts.MarchMadness.deployed()
            .then(function(instance) {
                MM = instance;
                var storableBracket = this.hashBracket(bracket);
                return MM.payWiner(winner, poolId, storableBracket, {from: winner, gasPrice: MMStore.gasPrice, gas: MMStore.maxGas})
            })
            .then(function(success) {
                if(success) {
                    console.log('Claim Paid');
                } else {
                    console.log('Bracket Update Completed but not saved');
                };
            })
            .catch(function(err) {
                console.log(err)
            });
        },
        payWinnerFromAccount: function(winner, poolId, bracket, sender) {
            var MM;
            MarchMadnessApp.contracts.MarchMadness.deployed()
            .then(function(instance) {
                MM = instance;
                var storableBracket = this.hashBracket(bracket);
                return MM.updateBracket(winner, storableBracket, {from: sender, gasPrice: MMStore.gasPrice, gas: MMStore.maxGas})
            })
            .then(function(success) {
                if(success) {
                    console.log('Claim Paid from :' + sender);
                } else {
                    console.log('Bracket Update Completed but not saved');
                };
            })
            .catch(function(err) {
                console.log(err)
            });
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
        withdraw: function(amount, recipient, approvingAdmin) {
            var MM;
            MarchMadnessApp.contracts.MarchMadness.deployed()
            .then(function(instance) {
                MM = instance;
                return MM.withdrawFunds(recipient, amount, {from: approvingAdmin})
            })
            .then(function(success) {
                if(success) {
                    console.log('Withdral completed');
                } else {
                    console.log('Bracket Update Completed but not saved');
                };
            })
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
  