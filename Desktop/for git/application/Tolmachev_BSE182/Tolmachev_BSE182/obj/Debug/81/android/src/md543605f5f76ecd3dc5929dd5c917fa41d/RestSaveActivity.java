package md543605f5f76ecd3dc5929dd5c917fa41d;


public class RestSaveActivity
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("Tolmachev_BSE182.RestSaveActivity, Tolmachev_BSE182", RestSaveActivity.class, __md_methods);
	}


	public RestSaveActivity ()
	{
		super ();
		if (getClass () == RestSaveActivity.class)
			mono.android.TypeManager.Activate ("Tolmachev_BSE182.RestSaveActivity, Tolmachev_BSE182", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
