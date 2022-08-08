using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoMono
{
	public abstract class BasePageContext : BaseContext
	{

	}

	public abstract class BasePageContext<Page, MonoPage> : BaseContext
		where Page : BasePageContext<Page, MonoPage>
		where MonoPage : BaseMonoPageContext 
		
	{
		protected MonoPage _mono;

		private static Page _pageContext;
		public static Page Instance => _pageContext;
		
		protected override void Init()
		{
			_mono = _monoContext as MonoPage;

			_pageContext = this as Page;
		}
	}
}