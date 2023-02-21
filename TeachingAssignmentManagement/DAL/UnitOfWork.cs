using System;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.DAL
{
    public class UnitOfWork : IDisposable
    {
        private readonly CP25Team03Entities context;
        private AcademicDegreeRankRepository academicDegreeRankRepository;
        private AcademicDegreeRepository academicDegreeRepository;
        private ClassSectionRepository classSectionRepository;
        private CoefficientRepository coefficientRepository;
        private LecturerRankRepository lecturerRankRepository;
        private MajorRepository majorRepository;
        private RoomRepository roomRepository;
        private SubjectRepository subjectRepository;
        private TermRepository termRepository;
        private UnitPriceRepository unitPriceRepository;
        private UserRepository userRepository;

        public UnitOfWork(CP25Team03Entities context)
        {
            this.context = context;
        }

        public AcademicDegreeRankRepository AcademicDegreeRankRepository
        {
            get
            {
                if (academicDegreeRankRepository == null)
                {
                    academicDegreeRankRepository = new AcademicDegreeRankRepository(context);
                }
                return academicDegreeRankRepository;
            }
        }

        public AcademicDegreeRepository AcademicDegreeRepository
        {
            get
            {
                if (academicDegreeRepository == null)
                {
                    academicDegreeRepository = new AcademicDegreeRepository(context);
                }
                return academicDegreeRepository;
            }
        }

        public ClassSectionRepository ClassSectionRepository
        {
            get
            {
                if (classSectionRepository == null)
                {
                    classSectionRepository = new ClassSectionRepository(context);
                }
                return classSectionRepository;
            }
        }

        public CoefficientRepository CoefficientRepository
        {
            get
            {
                if (coefficientRepository == null)
                {
                    coefficientRepository = new CoefficientRepository(context);
                }
                return coefficientRepository;
            }
        }

        public LecturerRankRepository LecturerRankRepository
        {
            get
            {
                if (lecturerRankRepository == null)
                {
                    lecturerRankRepository = new LecturerRankRepository(context);
                }
                return lecturerRankRepository;
            }
        }

        public MajorRepository MajorRepository
        {
            get
            {
                if (majorRepository == null)
                {
                    majorRepository = new MajorRepository(context);
                }
                return majorRepository;
            }
        }

        public RoomRepository RoomRepository
        {
            get
            {
                if (roomRepository == null)
                {
                    roomRepository = new RoomRepository(context);
                }
                return roomRepository;
            }
        }

        public SubjectRepository SubjectRepository
        {
            get
            {
                if (subjectRepository == null)
                {
                    subjectRepository = new SubjectRepository(context);
                }
                return subjectRepository;
            }
        }

        public TermRepository TermRepository
        {
            get
            {
                if (termRepository == null)
                {
                    termRepository = new TermRepository(context);
                }
                return termRepository;
            }
        }

        public UnitPriceRepository UnitPriceRepository
        {
            get
            {
                if (unitPriceRepository == null)
                {
                    unitPriceRepository = new UnitPriceRepository(context);
                }
                return unitPriceRepository;
            }
        }

        public UserRepository UserRepository
        {
            get
            {
                if (userRepository == null)
                {
                    userRepository = new UserRepository(context);
                }
                return userRepository;
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}