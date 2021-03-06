﻿using System.Web;
using System.Web.Optimization;

namespace March_Madness
{
	public class BundleConfig
	{
		// For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
						"~/Scripts/jquery-{version}.js"));

			bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
						"~/Scripts/jquery.validate*"));

			bundles.Add(new ScriptBundle("~/bundles/bracketScripts").Include(
						"~/Scripts/utility.js"));
			// Use the development version of Modernizr to develop with and learn from. Then, when you're
			// ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
			bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
						"~/Scripts/modernizr-*"));

			bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
					  "~/Scripts/bootstrap.js",
					  "~/Scripts/respond.js"));
		    bundles.Add(new ScriptBundle("~/bundles/truffle-contracts").Include(
                "~/bower_components/web3/dist/web3.min.js",
                "~/node_modules/truffle-contract/dist/truffle-contract.js",
                "~/Contracts/scripts/MarchMadness.js"
                ));

			bundles.Add(new StyleBundle("~/Content/css").Include(
					  "~/Content/bootstrap-cyborg.css",
					  "~/Content/site.css"));
                   
		}
	}
}
