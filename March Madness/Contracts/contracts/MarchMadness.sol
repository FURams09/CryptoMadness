pragma solidity ^0.4.4;

contract MarchMadness {
  address Owner;
  event OwnershipUpdated(address oldOwner, address newOwner);

  uint LockTime;

  uint public EntryFee; //what we take
  uint public PoolFee; //what we take
  uint public SalesCommission; //

  mapping(address => bytes32) AdminPasswords;
  mapping(address => bytes32) TempAdminPasswords;

  mapping(bytes32 => uint) PoolFees; //poolId to entry fee in Wei
  mapping(bytes32 => uint) PoolValue; //poolId to poolsTotalEntryFeeCollected
  mapping(bytes32 => bytes32) PoolOwnerPasswords; //an pool to a hash of the pool and the Pool Owner's password
  mapping(string => bytes32) PoolPublicNames; //Pools Given Public Names to Look them Up
  mapping(bytes32 => uint8) MaxPoolEntriesPerAddress; //Max Entries Per Address in each pool
  mapping(bytes32 => uint8) PoolEntriesPerAddress; //Max Entries Per Address in each pool
  mapping(bytes32 => uint) MaxPoolEntries; //total entries in each pool
  mapping(bytes32 => uint) PoolEntants; //total entries in each pool
  

  mapping(bytes32 => bytes32) BracketStore; //EntryKey to bracket
  mapping(bytes32 => address) BracketOwnerPasswords; //maps an EntryKey to its owner
  mapping(bytes32 => bytes32) BracketEscrow; //Maps the NewOwner/TempPassword with the Bracket they're buying

  
  uint TotalPayouts;
  uint TotalCharity; 


  event PaymentMade(address payee, address payer, string paymentMsg);
  
  event PaymentError(address attemptedPayee, address attemptedPayer, string errMsg);
  event BracketError(address entrant, string errMsg);
  event SaleError(address errParty, string errMsg);


  event PoolCreated(address poolPublicAddress, string poolPublicName, uint fee);
  event BracketCreated(address owner, uint indexed poolId, bytes32 bracket);

  mapping(address => uint) charityHold;
  event DonationReceived(address donor, uint amountDonated);

  event AdminError(address attemptAddress, string errMsg);
  event AdminMessage(string message);

  modifier isBracketOwner(string poolAddress, string poolPassword, string bracketPassword, bytes32 ) {
    var pool = getBracketOwnerHash(msg.sender, poolPassword);
    require pool == BracketOwnerPasswords[] ;
    _;
  }

  modifier isAdmin(string password) {
    require(password != "");
    require(AdminPasswords[msg.sender] == getPasswordHash(msg.sender, password));
    _;
  }

  modifier tournamentNotStarted() {
    require(now <= LockTime);
    _;
  }


  function MarchMadness(uint poolFee, uint lockBracketsAfter, uint entryFee, uint16 salesCommission, string password) public {
    require(bytes(password).length > 8);
    AdminPasswords[msg.sender] = getPasswordHash(msg.sender, password);
    PoolFee = poolFee;
    EntryFee = entryFee;
    SalesCommission = salesCommission;
    LockTime = now + lockBracketsAfter;
    AdminMessage("Contract Deployed");
  }

  function addAdmin(address newAdmin, string tempPassword, string password) public isAdmin(password) {
	  require(newAdmin != 0x0);
    TempAdminPasswords[msg.sender] = getPasswordHash(newAdmin, tempPassword);
    //this will also reset password
  }

  function setAdminPassword(string tempPassword, string newPassword) public {
    require(TempAdminPasswords[msg.sender] == getPasswordHash(msg.sender, tempPassword));
    require(bytes(newPassword).length > 8);
    AdminPasswords[msg.sender] = getPasswordHash(msg.sender, newPassword);
    TempAdminPasswords[msg.sender] == 0x0;
  }

  function removeAdmin(address removedAdmin, string password) public isAdmin(password) {
    require(msg.sender != removedAdmin); //can't remove self from admin
    AdminPasswords[removedAdmin] = 0x0;
  }

  function updateEntryFee(uint newFee, string password) public isAdmin(password) { 
    EntryFee = newFee;
  }

  function updatePoolFee(uint newFee, string password) public isAdmin(password) {
    PoolFee = newFee;
  }

  function updateSalesCommission(uint newFee, string password) public isAdmin(password) {
    SalesCommission = newFee;
  }
  
  function secondsUntilLock() public view returns (uint secondsLeft) {
	  return LockTime - now;
  }

  

  function getContractValue(string password) public view isAdmin(password) returns (uint balance, uint payoutsDue, uint charity) {
  	  return (this.balance, TotalPayouts, TotalCharity);
  }

  function withdrawFunds(address withdrawTo, uint amount, string password) public isAdmin(password) returns (bool) {
    if (this.balance - (amount) < TotalPayouts) {
      PaymentError(withdrawTo, msg.sender, "Not Enough Funds in Contract to Cover TotalPayout");
      return false;
    }
    if (this.balance - (amount) > this.balance) {
      PaymentError(withdrawTo, msg.sender, "Underflow of account");
      return false;
    }

    if (withdrawTo == 0x0) { 
      PaymentError(withdrawTo, msg.sender, "noWithdrawTo address");
      return false;
    }
    withdrawTo.transfer(amount);
    PaymentMade(withdrawTo, msg.sender, "Administrator Withdrawl");
    return true;
  }


  function createPool(uint fee, uint bonus, uint8 maxEntries, uint8 maxPerAddress, string publicName, string poolPassword, string ownerPassword) public payable tournamentNotStarted {
    var poolHash = getPasswordHash(msg.sender, poolPassword);
    require(PoolOwnerPasswords[poolHash] == 0x0); //if this owner has used password before, don't let them reuse it. 
    require(bytes32(poolPassword).length > 8);
    require(bytes32(ownerPassword).length > 8);
    if (msg.value < PoolFee) {
      PaymentError(msg.sender,  "Insufficient fee to cover pool creation");
      return;
    }
    PoolFees[poolHash] = fee;
    PoolValue[poolHash] = bonus;
    MaxPoolEntriesPerAddress[poolHash] = maxPerAddress; //Max Entries Per Address in each pool
    MaxPoolEntries[poolHash] = maxEntries; //total entries in each pool
    PoolPublicNames[getPublicPoolHash(msg.sender, publicName)] = poolHash; //makes a unique identifier of the Public Address and the Name. Makes Names not have to be unique, but still lookupable
    PoolCreated(msg.sender, PoolFees[msg.sender]);
  }

  function getPoolValue(address poolAddress, string publicName) public view returns (uint) {
    var poolKey = getPublicPoolHash(poolAddress, publicName);
    return PoolValue[poolKey];
  }

  function getPoolEntryFee(address poolOwnerAddress, string poolName) public view returns(uint) {
    var poolHash = getPoolHash(poolOwnerAddress, poolName);
    return PoolFees[poolHash];
  }


  //The Entrant will Need to Know the Pool Owner's Address and the Password for that Pool
  function createBracket(bytes32 bracket, address poolAddress, string poolPassword, string bracketPassword)  public payable tournamentNotStarted {
    var poolHash = getPoolHash(poolAddress, poolPassword);
    uint poolEntryFee = PoolFees[poolHash];

    if (msg.value < poolEntryFee + EntryFee) {
      BracketError(msg.sender,  "Pool and Entry fee not met");
      return;
    }
    require(bytes(bracketPassword).length > 8);
    uint poolValue = PoolValue[poolHash];
    if (poolValue + poolEntryFee < poolValue) {
      BracketError(msg.sender, "Pool value would overflow with more brackets");
      return;
    }

    var entryKey = getBracketOwnerHash(poolAddress, poolPassword, bracketPassword);
    BracketStore[entryKey] = bracket;
    BracketPassword[]
    TotalPayouts += poolEntryFee;
    PoolValue[poolHash] += poolEntryFee;
    BracketCreated(msg.sender, poolAddress);
  }


  function updateBracket(bytes32 bracket, address poolAddress, string poolPassword, string bracketPassword) public tournamentNotStarted {
    var entryKey = getBracketOwnerHash(poolAddress, poolPassword, bracketPassword);
    require(BracketStore[entryKey] != 0);
    BracketStore[entryKey] = bracket;
  }


  function payWinner(address winner, uint16 pool, bytes32 bracket, string password)  public payable isAdmin(password) returns(bool) {
      if (bracket == 0) {
        PaymentError(winner, msg.sender, "No Bracket to Validate");
        return false;
      }
      if (BracketStore[winner] != bracket) {
        PaymentError(winner, "Final and Submitted Bracket Not Equal");
        return false;
      }


      require(PoolValue[pool] > 0);
      winner.transfer(PoolValue[pool]);
      PaymentMade(winner, msg.sender, "Winner Paid");
      TotalPayouts -= PoolFees[pool];
      PoolValue[pool] = 0;
  }

  function getPublicPoolHash(address poolAddress, string name) private pure returns(bytes32) {
    return sha3(poolAddress, name);
  }
  function getBracketOwnerHash(address poolAddress, string poolPassword, string bracketPassword) private pure returns (bytes32) {
    return sha3(poolAddress, poolPassword, bracketPassword, msg.sender);
  }

  function getPasswordHash(string password) private view returns (bytes) {
    return keccak256(msg.sender, password);
  }

  function getPoolHash(address poolAddress, string poolPassword) private pure returns (bytes32) {
    return keccak256(poolAddress, poolPassword);
  }

  function () public payable {
     //if something goes wrong put money in the charity hold for either a later fund or if unclaimed keep/give to charity
    charityHold[msg.sender] += msg.value;
    TotalCharity += msg.value;
    DonationReceived(msg.sender, msg.value);
  }

  // function refundDonation(address refundAddress, uint amountToRefund) public returns (bool) {
  //   require(this.balance >= TotalPayouts + amountToRefund);
  //   if (charityHold[refundAddress] > 0) {
  //     refundAddress.transfer(amountToRefund);
  //     DonationRefunded(refundAddress, amountToRefund, charityHold[refundAddress]);
  //   } else {
  //     return false;
  //   }
  // }
}
