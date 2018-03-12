var MarchMadness = artifacts.require("./MarchMadness.sol");
module.exports = function(deployer) {
  deployer.deploy(MarchMadness, web3.toWei(1, 'ether'), 10000000000, web3.toWei(0.05), web3.toWei(0.6));
};
