﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Models
{
    
    public class Article
    {
        

        [Key]
        public int Id { set; get; }
        [StringLength(255,MinimumLength =5,ErrorMessage ="{0} phải dài từ {2} đến {1} ký tự ")]
        [Required]
        [Column(TypeName = "nvarchar")]
        [DisplayName("Tiêu đề")]
        public string Title { set; get; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage ="{0} phải nhập")]
        [DisplayName("Ngày tạo")]
        public DateTime Created { set; get; }
        
        [Column(TypeName = "ntext")]
        [DisplayName("Nội dung")]
        public string Content { set; get; }


    }
}
