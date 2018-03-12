var MarchMadness = artifacts.require("./MarchMadness.sol");
module.exports = function(deployer) {
  deployer.deploy(MarchMadness, 1, 10000000000, 0.05, 0.6);
};
