pragma solidity ^0.4.18;

import "./ConvertLib.sol";

// This is just a simple example of a coin-like contract.
// It is not standards compatible and cannot be expected to talk to other
// coin/token contracts. If you want to create a standards-compliant
// token, see: https://github.com/ConsenSys/Tokens. Cheers!

contract MetaCoin {
	mapping (address => uint) balances;
	uint public costPerMetaCoin = 100;
	address owner;

	event Minted(address indexed _to, uint indexed _amount);
	event Bought(address indexed _buyer, uint indexed _amountSent, uint indexed _amountMinted);
	event Transfer(address indexed _from, address indexed _to, uint256 _value);
	
	modifier onlyOwner() {
		require(msg.sender == owner);
		_;
	}

	function MetaCoin() public {
		balances[msg.sender] = 10000;
		owner == msg.sender;
	}

	function setRate(uint costPerCoin) public onlyOwner {
		costPerMetaCoin = costPerCoin;
	}

	function createCoin(address receiver, uint amt) public onlyOwner {
		balances[receiver] += amt;
		Minted(receiver, amt);
	}
	function sendCoin(address receiver, uint amount) public returns(bool sufficient) {
		if (balances[msg.sender] < amount) {return false;}
		balances[msg.sender] -= amount;
		balances[receiver] += amount;
		Transfer(msg.sender, receiver, amount);
		return true;
	}

	function() public payable {
		uint boughtAmount = ConvertLib.convert(weiToEth(msg.value), costPerMetaCoin);
		balances[msg.sender] += boughtAmount;
		Bought(msg.sender, msg.value,  boughtAmount);
	}

	function weiToEth(uint val) private pure returns (uint) {
		return val / 1 ether;
	}

	function getContractValue() public view onlyOwner returns (uint) {
		return this.balance;
	}

	function withdrawlEther(uint amt) public payable onlyOwner {
		msg.sender.transfer(amt);
	}

	function getBalanceInEth(address addr) public view returns(uint) {
		return ConvertLib.convert(getBalance(addr),costPerMetaCoin);
	}

	function getBalance(address addr) public view returns(uint) {
		return balances[addr];
	}
}
