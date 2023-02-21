namespace TeachingAssignmentManagement.Models
{
    public class CoefficientDTO
    {
        public int Id { get; set; }
        public decimal VietnameseCoefficient { get; set; }
        public decimal ForeignCoefficient { get; set; }
        public decimal TheoreticalCoefficient { get; set; }
        public decimal PracticeCoefficient { get; set; }
    }
}