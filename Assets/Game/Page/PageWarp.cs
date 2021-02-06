using System.Collections;
using UnityEngine;

public class PageWarp : PageAutoNavigation
{
    public override void OnActivate()
    {
        base.OnActivate();
        StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        yield return new WaitForSeconds(2f);
        Skip();
    }

    public void Skip()
    {
        //Debug.Log("Skip");
        StopAllCoroutines();
        pageManager.PopPage();
    }
}
