using System.Diagnostics.CodeAnalysis;

namespace wpfCodeCheck.Main.Local.Models
{
    public class CodeInfoCompareer : IEqualityComparer<CodeInfoModel>
    {
        public bool Equals(CodeInfoModel? x, CodeInfoModel? y)
        {
            if (ReferenceEquals(x, null) || ReferenceEquals(y,null))
                return false;
            return (x.Checksum == y.Checksum)
                && (x.FileName == y.FileName)
                && (x.CreateDate == y.CreateDate);
        }

        public int GetHashCode([DisallowNull] CodeInfoModel obj)
        {
            return (obj.Checksum, obj.FileName, obj.CreateDate).GetHashCode();
        }
    }
}
