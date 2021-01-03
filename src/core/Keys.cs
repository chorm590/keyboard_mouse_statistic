
namespace KMS.src.core
{
    class Keys
    {
        internal int Code;
        internal string PrimaryFun;
        internal string SecondaryFun;
        internal string DescEn;
        internal string DescCn;

        internal Keys(int code, string priFun, string descEn, string descCn)
        {
            Code = code;
            PrimaryFun = priFun;
            DescEn = descEn;
            DescCn = descCn;
        }

        internal Keys(int code, string priFun, string secFun, string descEn, string descCn)
        {
            Code = code;
            PrimaryFun = priFun;
            SecondaryFun = secFun;
            DescEn = descEn;
            DescCn = descCn;
        }
    }
}
