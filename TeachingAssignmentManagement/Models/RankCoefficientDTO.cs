namespace TeachingAssignmentManagement.Models
{
    public class RankCoefficientDTO
    {
        public int Id { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal VietnameseCoefficient { get; set; }
        public decimal ForeignCoefficient { get; set; }
        public string AcademicDegreeRankId { get; set; }
    }
}