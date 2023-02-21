﻿using System.Collections.Generic;
using System.Linq;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.DAL
{
    public class CoefficientRepository
    {
        private readonly CP25Team03Entities context;

        public CoefficientRepository(CP25Team03Entities context)
        {
            this.context = context;
        }

        public coefficient GetRankCoefficients(int startYear, int endYear)
        {
            return context.coefficients.SingleOrDefault(r => r.start_year == startYear && r.end_year == endYear);
        }

        public coefficient GetRankCoefficientByID(int id)
        {
            return context.coefficients.Find(id);
        }

        public void InsertRankCoefficient(coefficient rankCoefficient)
        {
            context.coefficients.Add(rankCoefficient);
        }

        public void DeleteAllRankCoefficients(int startYear, int endYear)
        {
            context.coefficients.RemoveRange(context.coefficients.Where(r => r.start_year == startYear && r.end_year == endYear));
        }
    }
}