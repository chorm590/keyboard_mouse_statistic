
namespace KMS.src.core
{
    class Keys
    {
        internal int Code;
        internal string PriDescEn;
        internal string PriDescCn;
        internal string SecDescEn;
        internal string SecDescCn;

        internal Keys(int code, string priDescEn, string priDescCn)
        {
            Code = code;
            PriDescEn = priDescEn;
            PriDescCn = priDescCn;
        }

        internal Keys(int code, string priDescEn, string priDescCn, string secDescEn, string secDescCn)
        {
            Code = code;
            PriDescCn = priDescCn;
            PriDescEn = priDescEn;
            SecDescEn = secDescEn;
            SecDescCn = secDescCn;
        }
    }
}
