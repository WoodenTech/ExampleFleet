using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BDElite.Fleet.API.Models
{
    public class Vehicle
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("make")]
        public string Make { get; set; } = string.Empty;

        [BsonElement("model")]
        public string Model { get; set; } = string.Empty;

        [BsonElement("year")]
        public int Year { get; set; }

        [BsonElement("licensePlate")]
        public string LicensePlate { get; set; } = string.Empty;

        [BsonElement("vin")]
        public string VIN { get; set; } = string.Empty;

        [BsonElement("status")]
        public VehicleStatus Status { get; set; } = VehicleStatus.Available;

        [BsonElement("mileage")]
        public int Mileage { get; set; }

        [BsonElement("lastServiceDate")]
        public DateTime? LastServiceDate { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum VehicleStatus
    {
        Available,
        InUse,
        Maintenance,
        OutOfService
    }
}