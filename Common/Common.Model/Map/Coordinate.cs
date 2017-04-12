namespace Common.Model.Map
{
    public class Coordinate
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Coordinate()
        {
            Latitude = 0;
            Longitude = 0;
        }

        public Coordinate(double latitude, double longitude) : this()
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public override bool Equals(object obj)
        {
            if (obj is Coordinate)
            {
                var other = obj as Coordinate;
                return this.Latitude == other.Latitude && this.Longitude == other.Longitude;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
