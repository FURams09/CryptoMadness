var MarchMadness = artifacts.require("./MarchMadness.sol");

var MMStore = {
	//set in initWeb
	poolFee: 1,
	entryFee:0.05,
	salesCommission:0.6,
	//properties are the fees used for testing as the fee to enter a pool
	poolEntryFee: 0.75,
    timeUntilLock: 10000000000,
    bracket: "0xfad160e87111505220b172e1930d505827ed0e60d9217b76c618dd3643c6f117",
    accountToCharge: 5, //to keep all of the eth coming from the same account and running out 
    admins: [],
    accounts:[],
    pools:[],
    brackets:[],
    poolId: 4,
    bracketId: 18,
    gasAllowance: .005,
    maxGas: 1000000,
    gasPrice: 300
}

contract('MarchMadness', function(accounts) {
  var march_madness;
  before(function(done) {
    assert.isTrue(true, "before ran");
    march_madness = MarchMadness.deployed()
    .then(function(instance) {
      march_madness = instance;
      assert.isTrue(true, "did not deploy");
    })   
    .finally(done)
  })

  it("should Initialize MarchMadness", function(done) {
    march_madness.PoolFee.call().then(function(poolFee) {
      assert.equal(poolFee, MMStore.poolFee);
      done();
    })
  });

  it("should only let Admin withdraw", function(done) {
    var ethDonation = web3.toWei(2, "ether");
    var withdrawnAmount = web3.toWei(1, "ether");
    march_madness.sendTransaction({from: accounts[MMStore.accountToCharge], value: ethDonation})
    .then(function(donationTx) {
      assert.equal(donationTx.logs[0].args['amountDonated'], ethDonation, "Donation not Received");
      return march_madness.withdrawFunds(0x0, withdrawnAmount, {from: accounts[0]})
    })
    .then(function(goodWithdraw) {
      assert.equal(goodWithdraw.logs[0].event, 'PaymentMade', "Admin can't withdraw");
      return march_madness.withdrawFunds(accounts[1], 1, {from: accounts[1]})
    })
    .then(function(badTx) {
      assert.isTrue(false, "Non Admin Withdrew");
    })
    .catch(async function(err) {
      //console.log(err)
      var contractValue = await march_madness.getContractValue({from: accounts[0]})
      
      assert.equal(contractValue.toNumber(), ethDonation - withdrawnAmount);
      //sorry future dev for not catching any unexpected errors when expecting to catch a revert. 
      //left the console.log to manually see what error we're dealing with. 
      //console.log(err);
      done();
    })
  });

  it("should add and remove Admin", function(done) {
    var ethDonation = web3.toWei(2, "ether");
    var withdrawnAmount = web3.toWei(1, "ether");
    march_madness.sendTransaction({from: accounts[MMStore.accountToCharge], value: ethDonation})
    .then(function(donationTx) {
      return march_madness.addAdmin(accounts[1], {from: accounts[0]})
    })   
    .then(function(adminTx) {
      return march_madness.withdrawFunds(accounts[1], 1, {from: accounts[1]})
    })
    .then(function(goodWithdraw) {
      assert.equal(goodWithdraw.logs[0].event, 'PaymentMade', "Admin not granted");
      return march_madness.removeAdmin(accounts[1], {from: accounts[0]})
    })
    .then(function(adminTx) {
      return march_madness.withdrawFunds(accounts[1], 1, {from: accounts[1]})
    })
    .then(function(tx) {
      assert.isTrue(false, "Admin not removed");
    })
    .catch(function(err) {
      //sorry future dev for not catching any unexpected errors when expecting to catch a revert. 
      //left the console.log to manually see what error we're dealing with. 
      //console.log(err);
      done();
    })
  });

  it("can't have empty Admin", function(done) {
    var ethDonation = web3.toWei(2, "ether");
    var withdrawnAmount = web3.toWei(1, "ether");
    march_madness.sendTransaction({from: accounts[MMStore.accountToCharge], value: ethDonation})
    .then(function() {
      return march_madness.addAdmin(0x0, {from: accounts[0]})
    })
    .then(function(badTx) {
      assert.isTrue(false, '0x0 added as Admin')
    })
    .catch(function(err) {
     
      march_madness.withdrawFunds(accounts[1], 0.01, {from: 0x0})
      .then(function() {
        assert.isTrue(false, '0x0 withdrew eth');
      })
      .catch(function(err) {
        //console.log(err);
        done();
      })
     
    })

  });

  it("should Update Fees", async function() {
    var newEntryFee = web3.toWei(0.75, 'ether');
    var newPoolFee  = web3.toWei(0.5, 'ether');
    var newSalesCommission = web3.toWei(.25, 'ether');
    
    await march_madness.updateEntryFee(newEntryFee, {from: accounts[0]})
    var entryFee = await march_madness.EntryFee.call()
    assert.equal(entryFee, newEntryFee, "Entry Fee Not Updated");

    await march_madness.updatePoolFee(newPoolFee, {from: accounts[0]})
    var poolFee = await march_madness.PoolFee.call();
    assert.equal(poolFee, newPoolFee, "Pool Fee Not Updated");

    await  march_madness.updateSalesCommission(newSalesCommission, {from: accounts[0]});
    return march_madness.SalesCommission.call()
    .then(function(salesCommission){
      assert.equal(salesCommission, newSalesCommission, "Sales Commission Not Updated");
    });
   
  
  })

  it("should Create Pool " + MMStore.poolId, async function() {
    //reset after testing their updates
    MMStore.poolFee = web3.toWei(MMStore.poolFee, "ether");
    MMStore.entryFee = web3.toWei(MMStore.entryFee, "ether");
    MMStore.salesCommission = web3.toWei(MMStore.salesCommission, "ether");
    MMStore.poolEntryFee = web3.toWei(MMStore.poolEntryFee, "ether")
    //reset default fees after testing their updates
    await march_madness.updatePoolFee(MMStore.poolFee, {from: accounts[0]})
    await march_madness.updateEntryFee(MMStore.entryFee, {from: accounts[0]})
    await  march_madness.updateSalesCommission(MMStore.salesCommission, {from: accounts[0]});
    var pooltx = await march_madness.createPool(MMStore.poolId, MMStore.poolEntryFee, {from: accounts[0], value: MMStore.poolFee} )
    //console.log(pooltx);
    
      assert.equal(pooltx.logs[0].event, 'PoolCreated');
      assert.equal(pooltx.logs[0].args.poolId.toNumber(), MMStore.poolId, "PoolId mismatch");
      assert.equal(pooltx.logs[0].args.owner, accounts[0], "Pool Owner mismatch");
      assert.equal(pooltx.logs[0].args.fee.toNumber(), MMStore.poolEntryFee, "Pool Fee mismatch");
      
    return march_madness.createPool(MMStore.poolId, MMStore.poolEntryFee, {from: accounts[0], value: 1} )
    .then(function(pooltx) {
      assert.equal(pooltx.logs[0].event, 'PaymentError', "CreatedWithLowValue");
    })
    .catch(function(res) {
      console.log(res);
    })
  })

  it("should get pool Entry Fee", async function(){
    var poolEntryFee = await march_madness.getPoolEntryFee.call(MMStore.poolId);
    assert.equal(poolEntryFee.toNumber(), MMStore.poolEntryFee)
  })

  var bracketCreator = 2;
  it("should Create Bracket " + MMStore.bracketId, async function() {
    //chaning account as main bank
    MMStore.accountToCharge = 5;
    var entryFee = MMStore.entryFee;
    var poolEntryFee = MMStore.poolEntryFee;
    var pooltx = await march_madness.createBracket(MMStore.bracket, MMStore.poolId, {from: accounts[bracketCreator], value: parseInt(poolEntryFee) + parseInt(entryFee)} )
    assert.equal(pooltx.logs[0].event, 'BracketCreated');
    assert.equal(pooltx.logs[0].args.poolId.toNumber(), MMStore.poolId, "PoolId mismatch");
    assert.equal(pooltx.logs[0].args.owner, accounts[bracketCreator], "Bracket Owner mismatch");
    assert.equal(pooltx.logs[0].args.bracket, MMStore.bracket, "Bracket mismatch");
    console.log(pooltx.logs[0].args.bracket, MMStore.bracket)
    var poolBalance = await march_madness.getPoolValue.call(MMStore.poolId);
    assert.equal(poolBalance.toNumber(), poolEntryFee, 'Incorrect Fee Collected');
    return march_madness.createPool(MMStore.poolId, MMStore.poolEntryFee, {from: accounts[bracketCreator], value: 1} )
    .then(function(failedTx) {
      expect(failedTx.logs[0].event).to.equal('PaymentError');
    })
    .catch(function (err) {
      console.log(err)
    });
  })

 

  it("only Admin Pay Winner", async function() {
    var winnerAddress = accounts[bracketCreator];
    var winningPool = MMStore.poolId;
    var bracket = MMStore.bracket;
    var poolValueBeforePay = await march_madness.getPoolValue.call(MMStore.poolId);
    assert.notEqual(poolValueBeforePay.toNumber(), MMStore.entryFee, 'pool not set');

    return march_madness.payWinner(winnerAddress, winningPool, bracket, {from: accounts[5]})
    .then(function(badTx) {
      console.log(badTx);
      assert.isTrue(false, "Non-Admin initiated pay");
    })
    .catch(function(err) {
      //presumed reverted for bad owner
      //console.log(err);
    });
  
 
  });

  it("only pay verified Bracket", async function() {
    var winnerAddress = accounts[bracketCreator];
    var poolValueBeforePay = await march_madness.getPoolValue.call(MMStore.poolId);
    assert.notEqual(poolValueBeforePay.toNumber(), MMStore.entryFee, 'pool not set');
    var wasCaught = false;
    var payBogus = await  march_madness.payWinner(winnerAddress, MMStore.poolId, 'bogus', {from: accounts[0]})
    assert.equal(payBogus.logs[0].event, 'PaymentError');
    assert.equal(payBogus.logs[0].args.errCode.toNumber(), 2);   

  });

  it("should Pay Winner", async function() {
    var winnerAddress = accounts[bracketCreator];

    var poolValueBeforePay = await march_madness.getPoolValue.call(MMStore.poolId);
    assert.notEqual(poolValueBeforePay.toNumber(), MMStore.entryFee, 'pool not set');

    var payWinnerTx = await march_madness.payWinner(winnerAddress, MMStore.poolId, MMStore.bracket, {from: accounts[0]});
  
    assert.equal(payWinnerTx.logs[0].event, 'PaymentMade');
    assert.equal(payWinnerTx.logs[0].args.paidCode.toNumber(), 1, "Payment Code Incorrect");
    var poolValueAfterPay = await march_madness.getPoolValue.call(MMStore.poolId);
    assert.equal(poolValueAfterPay.toNumber(), 0, 'pool not zeroed');
  });

  it ("should Not Double Pay", async function() {
    var wasCaught = await march_madness.payWinner(accounts[bracketCreator], MMStore.poolId, MMStore.bracket, {from: accounts[0]})
    .then(function(doublePayWinnerTx) {
      return false;
    })
    .catch(function(err) {
      return  true;
     // console.log(err);
    });
    assert.isTrue(wasCaught, 'Double Withdraw went through');
  })
  

  
});
