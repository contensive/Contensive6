

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Contensive.BaseClasses
{
	/// <summary>
	/// The base class for all Contensive add-ons.
	/// </summary>
	/// <remarks>
	/// This is a sample addon created from the AddonBaseClass. To use this add-on, Start a new project and reference CPBase in your Contensive installation.
	/// Paste this code into a HelloWorld Class and build the DLL.
	/// Copy the DLL into the Contensive\Addon folder.
	/// Create a new collection on your site and name it Samples. (Manage Add-ons >> Advanced >> click on collections )
	/// Create a new Addon on your site, name it Hello World, set the Samples collection and set the Dot Net Class name to Contensive.Addons.HelloWorldClass
	/// Open the Navigator to Manage Add-ons >> Samples >> and click on Hello World
	/// 
	///<code >
	/// Namespace Contensive.Addons
	/// '
	/// 'Hello World
	/// '
	/// Public Class HelloWorldClass
	///     Inherits BaseClasses.AddonBaseClass
	///     Public Overrides Function Execute(ByVal CP As Contensive.BaseClasses.CPBaseClass) As Object
	///         Return "Hello World 2"
	///     End Function
	///End Class
	///End Namespace
	///  </code>
	/// </remarks>
	public abstract class AddonBaseClass
	{
		/// <summary>
		/// The only exposed method of an addon. Performs the functions for this part of the the add-on and returns an object, typically a string. For add-ons executing on a web page or as a remove method, the returned string is added to the page where the addon is placed. For addons run as processes, the returned string is logged in the process log.
		/// </summary>
		/// <param name="CP">An instance of the CPBaseClass with a valid CP.MyAddon object pointing to the current addon parameters (values for this addon in the database)</param>
		/// <returns></returns>
		/// <remarks></remarks>
		public abstract object Execute(BaseClasses.CPBaseClass CP);
	}
}

