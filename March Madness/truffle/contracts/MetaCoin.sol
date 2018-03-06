pragma solidity ^0.4.18;

import "./ConvertLib.sol";

// This is just a simple example of a coin-like contract.
// It is not standards compatible and cannot be expected to talk to other
// coin/token contracts. If you want to create a standards-compliant
// token, see: https://github.com/ConsenSys/Tokens. Cheers!

contract MetaCoin {
	mapping (address => uint) balances;
	uint public costPerMetaCoin = 100;
	event Transfer(address indexed _from, address indexed _to, uint256 _value);
	address owner;
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
	}
	function sendCoin(address receiver, uint amount) public returns(bool sufficient) {
		if (balances[msg.sender] < amount) return false;
		balances[msg.sender] -= amount;
		balances[receiver] += amount;
		Transfer(msg.sender, receiver, amount);
		return true;
	}

	function buyCoin() public payable returns(uint newBalance, uint coinsBought ) {
		coinsBought = (msg.value / 1 ether) * costPerMetaCoin;
		//balances[msg.sender] += coinsBought;

		newBalance =  balances[msg.sender];
	}

	function getBalanceInEth(address addr) public view returns(uint){
		return ConvertLib.convert(getBalance(addr),2);
	}

	function getBalance(address addr) public view returns(uint) {
		return balances[addr];
	}
}
