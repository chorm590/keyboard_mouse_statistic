namespace KMS.src.core
{
    /// <summary>
    /// 描述一个按键或一个组合键。
    /// </summary>
    class Type
    {
        internal int Code;
        internal string Desc;

        internal Type(int code, string desc)
        {
            Code = code;
            Desc = desc;
        }
    }
}