using System.Text.Json;

namespace MemeOfTheYear.Types
{
    public class Session
    {
        public String Id { get; set; } = "";

        public bool IsAuthenticated { get; set; }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is not Session) return false;

            return Equals((Session)obj);
        }

        public bool Equals(Session session)
        {
            return Id == session.Id;
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}