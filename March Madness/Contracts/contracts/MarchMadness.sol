pragma solidity ^0.4.4;

contract MarchMadness {
  address Owner;
  event OwnershipUpdated(address oldOwner, address newOwner);

  uint LockTime;

  uint public EntryFee; //what we take
  uint public PoolFee; //what we take
  uint public SalesCommission; //

  mapping(address => bool) ContractAdmins;

  mapping(uint16 => uint) Pools; //poolId to entry fee in Wei
  mapping(uint16 => uint) PoolValue; //poolId to poolsTotalEntryFeeCollected
  mapping(uint16 => address) PoolOwners;
  

  mapping(address => uint16) Entries;  //address to poolID
  mapping(address => bytes32) BracketStore; //address to bracket

  
  uint TotalPayouts;
  uint TotalCharity; 


  //0, "Authorized Withdraw
  //1, "Winner Payment"
  //2, Sale
  enum PaymentCode {
    AuthorizedWithdraw, 
    WinnerPayment, 
    Sale
    }
  event PaymentMade(address payee, address payer, PaymentCode indexed paidCode);
  
 

  // 0, "Insufficient value to cover sale"
  // 1, "No Bracket Passed for Validation"
  // 2, "Final and Submitted Bracket Not Equal"
  // 3, "Not Enough Funds in Contract to Cover TotalPayout"
  //4, "Entry Fee Not Met"
  //5, "Pool value at Overflow (currently uint72);
  //6, "Attempted to sell Empty bracket"
  //7, "Attempted to sell entry in pool with no payout
  enum ErrorCode {
    InsufficientValueForSale, 
    NoBracketToValidate,
    SubmittedAndFinalBracketMismatch, 
    PoolValueGreaterThanTotalPayout,
	PoolFeeNotMet,
    EntryFeeNotMet,
    PoolOverflow,
    SellingEmptyBracket,
    SellingFreeTournyBracket
    }
  event PaymentError(address attemptedPayee, ErrorCode indexed errCode, string errReason);
  event BracketError(address entrant, ErrorCode indexed errCode,string errReason);
  event SaleError(address errParty, ErrorCode indexed errCode, string errReason);
 


  event PoolCreated(address owner, uint indexed poolId, uint fee);
  event BracketCreated(address owner, uint indexed poolId, bytes32 bracket);
  mapping(address => uint) charityHold;
  event DonationReceived(address donor, uint amountDonated);
  event DonationRefunded(address donor, uint amountRefunded, uint donationLeft);


  modifier isPoolOwner(uint16 pool) {
    require(isAddressPoolOwner(msg.sender, pool));
    _;
  }

  modifier isAdmin() {
    require(ContractAdmins[msg.sender] == true);
    _;
  }

  modifier tournamentNotStarted() {
    require(now <= LockTime);
    _;
  }





  function MarchMadness(uint poolFee, uint lockBracketsAfter, uint entryFee, uint16 salesCommission) public {
    ContractAdmins[msg.sender] = true;
    PoolFee = poolFee;
    EntryFee = entryFee;
    SalesCommission = salesCommission;
    LockTime = now + lockBracketsAfter;
    
    OwnershipUpdated(0x0, Owner);
  }



  function addAdmin(address newAdmin) public isAdmin {
	require(newAdmin != 0x0);
    ContractAdmins[newAdmin] = true;
  }

  function removeAdmin(address removedAdmin) public isAdmin {
    require(msg.sender != removedAdmin);
    ContractAdmins[removedAdmin] = false;
  }

  function updateEntryFee(uint newFee) public isAdmin { 
    EntryFee = newFee;
  }

  function updatePoolFee(uint newFee) public isAdmin {
    PoolFee = newFee;
  }

  function updateSalesCommission(uint newFee) public isAdmin {
    SalesCommission = newFee;
  }
  
  function secondsUntilLock() public view returns (uint secondsLeft) {
	return LockTime - now;
  }

  function getContractValue() public view isAdmin returns (uint) {
  	  return this.balance;
  }

  //handled
  function withdrawFunds(address withdrawTo, uint amount) public isAdmin returns (bool) {
    if (this.balance - (amount) < TotalPayouts) {
      PaymentError(withdrawTo, ErrorCode.PoolValueGreaterThanTotalPayout, "Not Enough Funds in Contract to Cover TotalPayout");
      return false;
    }

    if (withdrawTo == 0x0) { 
      withdrawTo = Owner;
    }
    withdrawTo.transfer(amount);
    PaymentMade(withdrawTo, address(this), PaymentCode.AuthorizedWithdraw);
  }


  function createPool(uint16 pool, uint fee) public payable tournamentNotStarted {
    require(PoolOwners[pool] == 0x0);
    if (msg.value < PoolFee) {
      PaymentError(msg.sender, ErrorCode.PoolFeeNotMet, "Insufficient fee to cover pool creation");
      return;
    }
    Pools[pool] = fee;
    PoolOwners[pool] = msg.sender;
    PoolCreated(PoolOwners[pool], pool, Pools[pool]);
  }

  function getPoolValue(uint16 pool) public view returns (uint) {
    //require (isAddressContractAdmin(msg.sender) || isAddressPoolOwner(msg.sender, pool) || isAddressInPool(msg.sender, pool));
    return PoolValue[pool];
  }

  function getPoolEntryFee(uint16 pool) public view returns(uint) {
    return Pools[pool];
  }

  //eventually pass the hashed bracked
  function createBracket(bytes32 bracket, uint16 poolId)  public payable tournamentNotStarted {
    uint poolEntryFee = Pools[poolId];
    uint poolValue = PoolValue[poolId];
    if (msg.value < poolEntryFee + EntryFee) {
      BracketError(msg.sender, ErrorCode.EntryFeeNotMet, "Pool and Entry fee not met");
      return;
    }
    
    if (poolValue + poolEntryFee < poolValue) {
      BracketError(msg.sender, ErrorCode.PoolOverflow, "Pool value would overflow with more brackets");
      return;
    }

    Entries[msg.sender] = poolId;
    BracketStore[msg.sender] = bracket;
    TotalPayouts += msg.value - EntryFee;
    PoolValue[poolId] += poolEntryFee;
    BracketCreated(msg.sender, Entries[msg.sender],BracketStore[msg.sender]);
  }


  function updateBracket(bytes32 bracket) public tournamentNotStarted {
    require(Entries[msg.sender] != 0);
    BracketStore[msg.sender] = bracket;
  }
 

  function payWinner(address winner, uint16 pool, bytes32 bracket)  public payable isAdmin returns(bool) {
      if (bracket == 0) {
        PaymentError(winner, ErrorCode.NoBracketToValidate, "No Bracket Passed for Validation");
        return false;
      }
      if (BracketStore[winner] != bracket) {
        PaymentError(winner, ErrorCode.SubmittedAndFinalBracketMismatch, "Final and Submitted Bracket Not Equal");
        return false;
      }

      require(PoolValue[pool] > 0);
      winner.transfer(PoolValue[pool]);
      PaymentMade(winner, address(this), PaymentCode.WinnerPayment);
      TotalPayouts -= Pools[pool];
      PoolValue[pool] = 0;
  }

  function isAddressContractAdmin(address checkAddress) private view returns (bool) {
    return checkAddress == Owner;
  }

  function isAddressPoolOwner(address checkAddress, uint16 pool) private view returns (bool) {
    return PoolOwners[pool] == checkAddress;
  }

  function isAddressInPool(address checkAddress, uint16 pool) private view returns (bool) {
    return Entries[checkAddress] == pool;
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
