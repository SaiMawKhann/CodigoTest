namespace CodigoTest.Models
{
    public class MemberWithToken: Member
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public MemberWithToken(Member member)
        {
            this.MemberId = member.MemberId;
            this.MemberName = member.MemberName;
            this.Email = member.Email;
            this.MobileNumber = member.MobileNumber;
            this.Password = member.Password;
            this.MemberTotalPoint= member.MemberTotalPoint;
        }
    }
}
