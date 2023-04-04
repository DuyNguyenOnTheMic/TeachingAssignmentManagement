using TeachingAssignmentManagement.Helpers;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.Services
{
    public class RemunerationService
    {
        public static decimal CalculateRemuneration(class_section classSection, coefficient coefficient)
        {
            decimal crowdedClassCoefficient, timeCoefficient, languageCoefficient, classTypeCoefficient;

            // Check if class is theoretical or practice
            int studentNumber;
            if (classSection.type == MyConstants.TheoreticalClassType)
            {
                studentNumber = 50;
                classTypeCoefficient = coefficient.theoretical_coefficient;
            }
            else
            {
                studentNumber = 30;
                classTypeCoefficient = coefficient.practice_coefficient;
            }

            // Calculate crowded class coefficient
            int? studentRegistered = classSection.student_registered_number;
            crowdedClassCoefficient = studentRegistered <= studentNumber ? decimal.One : (decimal)(decimal.One + (studentRegistered - studentNumber) * 0.0025m);

            // Calculate time coefficient
            timeCoefficient = classSection.start_lesson_2 != 13 && classSection.day_2 != 8 ? decimal.One : 1.2m;

            // Calculate language coefficient
            languageCoefficient = classSection.subject.is_vietnamese ? coefficient.vietnamese_coefficient : coefficient.foreign_coefficient;

            // Calculate total remuneration for this class
            return crowdedClassCoefficient * timeCoefficient * classTypeCoefficient * languageCoefficient;
        }
    }
}