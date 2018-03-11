pragma solidity ^0.4.4;

contract MarchMadness {
  address Owner;
  event OwnershipUpdated(address oldOwner, address newOwner);

  uint LockTime;

  uint72 EntryFee; //what we take
  uint72 PoolFee; //what we take
  uint72 SalesCommission; //

  mapping(address => bool) ContractAdmins;

  mapping(uint16 => uint) Pools;
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
    EntryFeeNotMet,
    PoolOverflow,
    SellingEmptyBracket,
    SellingFreeTournyBracket
    }
  event PaymentError(address attemptedPayee, ErrorCode indexed errCode, string errReason);
  event BracketError(address entrant, ErrorCode indexed errCode,string errReason);
  event SaleError(address errParty, ErrorCode indexed errCode, string errReason);
 


  event PoolCreated(address owner, uint16 index, uint fee);
  mapping(address => uint) charityHold;
  event DonationReceived(address donor, uint amountDonated);
  event DonationRefunded(address donor, uint amountRefunded, uint donationLeft);


  modifier isPoolOwner(uint16 pool) {
    require(isAddressPoolOwner(msg.sender, pool));
    _;
  }

  modifier isAdmin() {
    require(ContractAdmins[msg.sender]);
    _;
  }

  modifier tournamentNotStarted() {
    require(now <= LockTime);
    _;
  }





  function MarchMadness(uint16 poolFee, uint lockBracketsAfter, uint16 entryFee, uint16 salesCommission) public {
    ContractAdmins[Owner] = true;
    PoolFee = poolFee * 1 ether;
    EntryFee = entryFee * 1 ether;
    SalesCommission = salesCommission * 1 ether;
    LockTime = now + lockBracketsAfter;
    
    OwnershipUpdated(0x0, Owner);
  }



  function transferOwner(address newOwner) public isAdmin {
    require(ContractAdmins[newOwner] == true);
    Owner = newOwner;
    OwnershipUpdated(msg.sender, Owner);
  }

  function addAdmin(address newAdmin) public isAdmin {
    ContractAdmins[newAdmin] = true;
  }

  function removeAdmin(address removedAdmin) public isAdmin {
    var adminToRemove memory = removeAdmin;
    require (msg.sender == removeAdmin);
    ContractAdmins[removedAdmin] = false;
  }


  function updateEntryFee(uint16 newFeeInEth) public isAdmin { 
    EntryFee = newFeeInEth * 1 ether;
  }

  function updatePoolFee(uint16 newFeeInEth) public isAdmin {
    PoolFee = newFeeInEth * 1 ether;
  }

  function updateSalesCommission(uint16 newFeeInEth) public isAdmin {
    SalesCommission = newFeeInEth * 1 ether;
  }


  function getContractBalance() isAdmin public returns(uint contractValueInEth, uint totalPayoutsDueInEth, uint totalCharityInEth) {
    contractValueInEth = this.balance / 1  ether;
    totalPayoutsDueInEth = TotalPayouts / 1 ether;
    totalCharityInEth = TotalCharity / 1 ether;

  }


  //handled
  function withdrawFunds(address withdrawTo, uint amountInEth) public isAdmin returns (bool) {
    if (this.balance - (amountInEth * 1 ether) < TotalPayouts) {
      PaymentError(withdrawTo, ErrorCode.PoolValueGreaterThanTotalPayout, "Not Enough Funds in Contract to Cover TotalPayout");
      return false;
    }

    if (withdrawTo == 0x0) { 
      withdrawTo = Owner;
    }
    withdrawTo.transfer(amountInEth * 1 ether);
    PaymentMade(withdrawTo, address(this), PaymentCode.AuthorizedWithdraw);
  }



  function createPool(uint16 pool, uint16 feeInEth) public payable tournamentNotStarted {
    require(PoolOwners[pool] == 0x0);
    require(msg.value >= PoolFee * 1 ether);
    Pools[pool] = feeInEth * 1 ether;
    PoolOwners[pool] = msg.sender;
    PoolCreated(msg.sender, pool, feeInEth);
  }

  function getPoolValueInEth(uint16 pool) public returns (uint) {
    require (isAddressContractAdmin(msg.sender) || isAddressPoolOwner(msg.sender, pool) || isAddressInPool(msg.sender, pool));
    return Pools[pool] / 1 ether;
  }
  //eventually pass the hashed bracked
  function createBracket(string bracket, uint16 pool)  public payable tournamentNotStarted returns(bool) {
    if (msg.value < Pools[pool] + EntryFee) {
      BracketError(msg.sender, ErrorCode.EntryFeeNotMet, "Pool and Entry fee not met");
      return false;
    }
    if (Pools[pool] + EntryFee > Pools [pool]) {
      BracketError(msg.sender, ErrorCode.PoolOverflow, "Pool value would overflow with more brackets");
      return false;
    }

    Entries[msg.sender] = pool;
    BracketStore[msg.sender] = keccak256(bracket);
    TotalPayouts += msg.value - EntryFee;
    return true;
  }


  function updateBracket(string bracket) public tournamentNotStarted {
    require(Entries[msg.sender] != 0);
    BracketStore[msg.sender] = keccak256(bracket);
  }
 

  // function sellBracket(address newOwner, uint saleAmountInEth) public payable returns (bool) {
  //   if (msg.value < (saleAmountInEth * 1 ether) + SalesCommission) {
  //     SalesError(msg.sender, ErrorCode.InsufficientValueForSale, "Insufficient value to cover sale");
  //     return false;
  //   }

  //   if (BracketStore[msg.sender] == 0x0) {
  //     SalesError(msg.sender, ErrorCode.SellingEmptyBracket, "Attempted to sell Empty Bracket");
  //     return false;
  //   }

  //   if (Pools[Entries[msg.sender]] == 0x0) {
  //     SalesError(msg.sender)
  //   }

  //   msg.sender.transfer(saleAmountInEth * 1 ether);
  //   Entries[newOwner] = Entries[msg.sender];
  //   BracketStore[newOwner] = BracketStore[msg.sender];
  //   Entries[msg.sender] = 0;
  //   BracketStore[msg.sender] = 0x0;
  //   PaymentMade(msg.sender, newOwner, PaymentCode.Sale);
  // }

  function payWinner(address winner, uint16 pool, bytes32 bracket)  public payable isAdmin returns(bool) {
      if (bracket == 0) {
        PaymentError(winner, ErrorCode.NoBracketToValidate, "No Bracket Passed for Validation");
        return false;
      }
      if (BracketStore[winner] != bracket) {
        PaymentError(winner, ErrorCode.SubmittedAndFinalBracketMismatch, "Final and Submitted Bracket Not Equal");
        return false;
      }

      if (Pools[pool] == 0) {
        PaymentError(winner, ErrorCode.PoolValueGreaterThanTotalPayout, "Pool Value GreaterThan");
        return false;
      }
      require(Pools[pool] > 0);
      winner.transfer(Pools[pool]);
      PaymentMade(winner, address(this), PaymentCode.WinnerPayment);
      TotalPayouts -= Pools[pool];
      Pools[pool] = 0;
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
