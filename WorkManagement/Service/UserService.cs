using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WorkManagement.Data;
using WorkManagement.DTO;
using WorkManagement.Interfaces;
using WorkManagement.Models;

namespace WorkManagement.Services
{
    public class UserService : IUserService
    {
        private readonly WorkManagementContext _context;

        public UserService(WorkManagementContext context)
        {
            _context = context;
        }

        //public async Task<User> AddUserAsync(User user, IFormFile avatarFile)
        //{
        //    // Xử lý lưu ảnh đại diện nếu có
        //    //if (avatarFile != null)
        //    //{
        //    //    var avatarPath = await SaveAvatarAsync(avatarFile);
        //    //    user.Avatar = avatarPath;
        //    //}
        //    if (avatarFile == null)
        //    {
        //        //var avatarPath = await SaveAvatarAsync(avatarFile);
        //        user.Avatar = null;
        //    }


        //    _context.Users.Add(user);
        //    await _context.SaveChangesAsync();
        //    return user;
        //}
        public async Task<User> AddUserAsync(User user, IFormFile avatarFile)
        {
            // Xử lý lưu ảnh đại diện nếu có
            if (avatarFile != null)
            {
                var avatarPath = await SaveAvatarAsync(avatarFile);
                user.Avatar = avatarPath;
            }

            // Tạo tên tài khoản và mật khẩu mặc định từ tên người dùng
            string username = RemoveDiacritics(user.Name).Replace(" ", "").ToLower(); // Tên tài khoản viết liền không dấu, chữ thường
            string password = username; // Mật khẩu mặc định là tên tài khoản

            // Tạo tài khoản cho người dùng
            var account = new Account
            {
                Username = username,
                Password = BCrypt.Net.BCrypt.HashPassword(password),  // Bạn có thể mã hóa mật khẩu ở đây nếu cần
                IsActive = true,       // Tài khoản mặc định là hoạt động
                RoleId = 2,            // Role mặc định là 2 (Nhân viên)
                User = user            // Gắn tài khoản với người dùng
            };

            // Lưu người dùng và tài khoản vào cơ sở dữ liệu
            _context.Users.Add(user);    // Thêm người dùng
            _context.Accounts.Add(account); // Thêm tài khoản
            await _context.SaveChangesAsync(); // Lưu thay đổi

            return user;
        }

        // Hàm loại bỏ dấu tiếng Việt trong tên
        private string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public async Task<User> UpdateUserAsync(int id, User user, IFormFile avatarFile)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (existingUser == null)
                throw new KeyNotFoundException("User not found.");

            // Cập nhật thông tin
            existingUser.Name = user.Name;
            existingUser.Email = user.Email;
            existingUser.Phone = user.Phone;
            existingUser.BirthDay = user.BirthDay;
            existingUser.DepartmentId = user.DepartmentId;
            existingUser.PositionId = user.PositionId;
            existingUser.IsLeader = user.IsLeader;
            existingUser.Avatar = existingUser.Avatar;
            // Cập nhật ảnh đại diện nếu có
            //if (avatarFile != null)
            //{
            //    if (!string.IsNullOrEmpty(existingUser.Avatar))
            //    {
            //        var oldAvatarPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", existingUser.Avatar);
            //        if (File.Exists(oldAvatarPath))
            //            File.Delete(oldAvatarPath);
            //    }

            //    existingUser.Avatar = await SaveAvatarAsync(avatarFile);
            //}

            await _context.SaveChangesAsync();
            return existingUser;
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            return user;
        }

        public async Task<IEnumerable<User>> GetUsersByNameAsync(string name)
        {
            return await _context.Users
                .Where(u => u.Name.Contains(name))
                .ToListAsync();
        }

        public async Task<User> UpdateAvatarAsync(int id, IFormFile avatarFile)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            // Thư mục chứa hình ảnh trên server
            var avatarFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

            // Tạo thư mục nếu chưa tồn tại
            if (!Directory.Exists(avatarFolder))
                Directory.CreateDirectory(avatarFolder);

            // Lưu tệp mới vào thư mục "Uploads"
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(avatarFile.FileName)}"; // Đảm bảo tên tệp là duy nhất
            var filePath = Path.Combine(avatarFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await avatarFile.CopyToAsync(stream);
            }

            // Xóa avatar cũ nếu tồn tại
            if (!string.IsNullOrEmpty(user.Avatar))
            {
                var oldAvatarPath = Path.Combine(avatarFolder, user.Avatar);
                if (File.Exists(oldAvatarPath))
                    File.Delete(oldAvatarPath);
            }

            // Cập nhật avatar mới
            user.Avatar = fileName;

            await _context.SaveChangesAsync();
            return user;
        }


        private async Task<string> SaveAvatarAsync(IFormFile avatarFile)
        {
            var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            Directory.CreateDirectory(uploadDirectory);

            var fileName = $"{Guid.NewGuid()}_{avatarFile.FileName}";
            var filePath = Path.Combine(uploadDirectory, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await avatarFile.CopyToAsync(stream);
            }

            return $"/Uploads/{fileName}";
        }


        public async Task UpdateDepartmentAsync(int userId, int departmentId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new Exception("Người dùng không tồn tại!");

            var department = await _context.Departments.FindAsync(departmentId);
            if (department == null)
                throw new Exception("Phòng ban không tồn tại!");

            user.DepartmentId = departmentId;
            await _context.SaveChangesAsync();
        }


        public async Task<IEnumerable<User>> GetUserByIsLeader()
        {
            // Lọc danh sách Users có IsLeader = 1
            var leaders = await _context.Users
                .Where(u => u.IsLeader == true)
                .ToListAsync();
            return leaders;
        }

        public async Task<IEnumerable<object>> GetAll()
        {
            var users = await _context.Users
                .Include(u => u.Department)
                .Include(u => u.Position)
                .Include(u => u.Notifications)
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    u.Phone,
                    u.BirthDay,
                    u.Avatar,
                    u.IsLeader,
                    u.Department,
                    u.Position,
                    Notifications = u.Notifications.Select(n => new
                    {
                        n.Id,
                        n.Title,
                        n.Content,
                        n.IsRead,
                        n.CreatedAt
                    })
                })
                .ToListAsync();

            return users;    

    }
        //Phân trang
        public async Task<object> GetAllPaged(int page, int pageSize)
        {
            var query = _context.Users
                .Include(u => u.Department)
                .Include(u => u.Position)
                .Include(u => u.Notifications)
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    u.Phone,
                    u.BirthDay,
                    u.Avatar,
                    u.IsLeader,
                    u.Department,
                    u.Position,
                    Notifications = u.Notifications.Select(n => new
                    {
                        n.Id,
                        n.Title,
                        n.Content,
                        n.IsRead,
                        n.CreatedAt
                    })
                });

            var totalCount = await query.CountAsync(); // Tổng số user
            var items = await query
                .Skip((page - 1) * pageSize) // Bỏ qua các phần tử trước đó
                .Take(pageSize)             // Lấy số lượng theo pageSize
                .ToListAsync();

            return new
            {
                TotalCount = totalCount, // Tổng số user
                Items = items            // Danh sách user cho trang hiện tại
            };
        }



        public async Task<List<User>> GetEmployeesForTaskAsync(int taskId)
        {
            var departmentId = await (from p in _context.Projects
                                      join pt in _context.ProjectTasks on p.Id equals pt.ProjectId
                                      where pt.Id == taskId
                                      select p.DepartmentId)
                                      .FirstOrDefaultAsync();

            if (departmentId == null)
            {
                return new List<User>(); // Không tìm thấy phòng ban
            }

            var employees = await _context.Users
                                          .Where(u => u.DepartmentId == departmentId &&
                                                     (u.IsLeader == false || u.IsLeader == null))
                                          .ToListAsync();

            return employees;
        }


        public async Task<User> GetUserbyAccountId(int accountId)
        {
            var account = await _context.Accounts
          .Include(a => a.User) // Join bảng User
          .FirstOrDefaultAsync(a => a.Id == accountId);
            return account?.User;
        }

        public async Task<IEnumerable<object>> StatisticalEmployeeOfDepartment()
        {
            var result = await _context.Departments
                .GroupJoin(
                    _context.Users.Where(u => u.IsLeader == false || u.IsLeader == null),
                    d => d.Id,
                    u => u.DepartmentId,
                    (department, employees) => new
                    {
                        DepartmentId = department.Id,
                        DepartmentName = department.Name,
                        EmployeeCount = employees.Count(),
                        Employees = employees
                            .OrderBy(e => e.Name)
                            .Select(e => new
                            {
                                UserId = e.Id,
                                UserName = e.Name,
                                Email = e.Email,
                                Phone = e.Phone,
                                BirthDay = e.BirthDay,
                                IsLeader = e.IsLeader
                            })
                            .ToList()
                    }
                )
                .OrderBy(g => g.DepartmentName) // Sắp xếp theo tên phòng ban
                .ToListAsync();

            return result;
        }

        //Thóng kê xem có bao nhiêu dự án hoàn thành và chưa
        public async Task<IEnumerable<object>> GetProjectStatusStatistics()
        {
            var result = await _context.Projects
                .Join(
                    _context.StatusJobs,
                    p => p.StatusId,
                    s => s.Id,
                    (p, s) => new
                    {
                        StatusName = s.Name
                    }
                )
                .GroupBy(x => x.StatusName)
                .Select(g => new
                {
                    Status = g.Key,
                    TotalProjects = g.Count()
                })
                .OrderBy(x => x.Status)
                .ToListAsync();

            return result;
        }
        public async Task<object> GetEmployeeTaskCompletionStatusWithCount()
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            // Truy vấn để lấy danh sách công việc và trạng thái hoàn thành
            var tasks = await _context.TaskEmployees
                .Join(
                    _context.Users,
                    te => te.UserId,
                    u => u.Id,
                    (te, u) => new { te, u }
                )
                .Join(
                    _context.ProjectTasks,
                    combined => combined.te.TaskId,
                    pt => pt.Id,
                    (combined, pt) => new
                    {
                        combined.te.UserId,
                        UserName = combined.u.Name,
                        combined.te.TaskId,
                        TaskName = pt.Name,
                        AssignedAt = combined.te.AssignedAt ?? DateTime.MinValue, // Kiểm tra và thay thế null
                        CompletionDate = combined.te.CompletionDate, // Không cần thay thế null, để xử lý sau
                        Deadline = combined.te.Deadline // Không cần kiểm tra null vì đã là kiểu DateTime
                    }
                )
                .Where(x => x.AssignedAt.Month == currentMonth && x.AssignedAt.Year == currentYear)
                .Select(x => new
                {
                    x.UserId,
                    x.UserName,
                    x.TaskId,
                    x.TaskName,
                    x.AssignedAt,
                    Status = x.CompletionDate == null
                        ? "Chưa hoàn thành"
                        : (x.CompletionDate > x.Deadline ? "Trễ hạn" : "Đúng hạn")
                })
                .ToListAsync();

            // Nhóm và đếm số lượng công việc theo trạng thái
            var statusCounts = tasks
                .GroupBy(task => task.Status)
                .Select(group => new
                {
                    Status = group.Key,
                    Count = group.Count() // Đếm số lượng công việc trong mỗi nhóm trạng thái
                })
                .ToList();

            // Kết quả cuối cùng bao gồm danh sách công việc và số lượng theo trạng thái
            var result = new
            {
                Tasks = tasks
                    .OrderBy(task => task.Status)  // Sắp xếp theo trạng thái
                    .ThenBy(task => task.UserName) // Sau đó theo tên nhân viên
                    .ToList(),
                StatusCounts = statusCounts
            };

            return result;
        }
        //Thống kê danh sách công việc thuộc 1 dự án
        public async Task<IEnumerable<object>> GetProjectTaskCountsAsync()
        {
            // Truy vấn SQL để đếm số lượng công việc (tasks) theo dự án (project)
            var result = await _context.Projects
                .GroupJoin(
                    _context.ProjectTasks,
                    p => p.Id,
                    pt => pt.ProjectId,
                    (p, tasks) => new
                    {
                        ProjectId=p.Id,
                        ProjectName = p.Name,
                        TaskCount = tasks.Count()  // Đếm số lượng công việc (tasks)
                    })
                .ToListAsync();

            return result.Cast<object>().ToList(); // Chuyển sang kiểu `object`
        }


    }
}

