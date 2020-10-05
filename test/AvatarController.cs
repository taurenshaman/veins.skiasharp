﻿//using Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using Common.Tools;

namespace Veins.Controllers {
  public class AvatarController : BaseController {
    //public ActionResult Index() {
    //  return View();
    //}


    [Route( "avatar/{username}/random" )]
    public ActionResult GetRandomAvatarOfUser(string username, int size = 256) {
      //BizHelper helper = BizHelper.Initialize();
      //var user = helper.SelectUserByUserName( username );
      //if (user == null) {
      //  username = "veins.design";
      //}

      if (size < 0)
        size = 256;
      else if (size > 1024)
        size = 1024;

      string timestamp = DateTime.UtcNow.ToString( "@yyyy-MM-dd-hh-mm-ss" );
      Veins.Art.Robohash robohash = new Art.Robohash( username + timestamp );

      string set = Veins.Art.Robohash.GetRandomSet();
      string color = Veins.Art.Robohash.GetRandomColor();
      string background = Veins.Art.Robohash.GetRandomBackground();
      var stream = robohash.Assemble( set, color, "png", background, size );

      return File( stream, "image/png" );
    }


    [Route( "avatar/{username}" )]
    public ActionResult GetAvatarOfUser(string username, int size = 256) {
      //BizHelper helper = BizHelper.Initialize();
      //var user = helper.SelectUserByUserName( username );
      //if(user == null) {
      //  username = "veins.design";
      //}

      if (size < 0)
        size = 256;
      else if (size > 1024)
        size = 1024;

      Veins.Art.Robohash robohash = new Art.Robohash( username );
      var stream = robohash.Assemble( "set1", "", "png", "", size );

      Response.Cache.SetExpires( DateTime.UtcNow );
      return File( stream, "image/png" );
    }

  }

}