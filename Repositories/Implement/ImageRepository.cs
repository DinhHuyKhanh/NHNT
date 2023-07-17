using System.Linq;
using NHNT.Constants.Statuses;
using NHNT.EF;
using NHNT.Exceptions;
using NHNT.Models;

namespace NHNT.Repositories.Implement
{
    public class ImageRepository : IImageRepository
    {
        private readonly DbContextConfig _context;

        public ImageRepository(DbContextConfig context)
        {
            _context = context;
        }

        public Image GetById(int id)
        {
            return _context.Images.SingleOrDefault(i => i.Id == id);
        }

        public void Add(Image image)
        {
            if (image == null)
            {
                throw new DataRuntimeException(StatusWrongFormat.IMAGE_IS_NULL);
            }

            _context.Images.Add(image);
            _context.SaveChanges();
        }

        public void Update(Image image)
        {
            if (image == null)
            {
                throw new DataRuntimeException(StatusWrongFormat.IMAGE_IS_NULL);
            }

            _context.Images.Update(image);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            Image image = this.GetById(id);
            if (image == null)
            {
                throw new DataRuntimeException(StatusNotExist.IMAGE_ID);
            }

            _context.Images.Remove(image);
            _context.SaveChanges();
        }
    }
}