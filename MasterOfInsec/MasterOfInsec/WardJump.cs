﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace MasterOfInsec
{
   static class WardJump
    {
       public static Vector3 posforward;
       public static float LastPlaced;
       public static float lastwardjump = 0;
       private static Obj_AI_Hero Player { get { return ObjectManager.Player; } }
       public static InventorySlot getBestWardItem()
       {
           InventorySlot ward = Items.GetWardSlot();
           if (ward == default(InventorySlot)) return null;
           return ward;
       }

       public static bool Harrasjump(Vector3 position)
       {
           #region ward ya existe
           if (Program.W.IsReady())
           {
               foreach (Obj_AI_Minion ward in ObjectManager.Get<Obj_AI_Minion>().Where(ward =>
                    ward.Name.ToLower().Contains("ward") && ward.Distance(Game.CursorPos) < 250))
               {
                   if (ward != null)
                   {
                       Program.W.CastOnUnit(ward);
                       Program.W.Cast();
                       return true;

                   }

               }

               foreach (
                   Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.Distance(Game.CursorPos) < 250 && !hero.IsDead))
               {
                   if (hero != null)
                   {
                       Program.W.CastOnUnit(hero);
                       Program.W.Cast();
                       return true;

                   }

               }

               foreach (Obj_AI_Minion minion in ObjectManager.Get<Obj_AI_Minion>().Where(minion =>
                   minion.Distance(Game.CursorPos) < 250))
               {
                   if (minion != null)
                   {
                       Program.W.CastOnUnit(minion);
                       Program.W.Cast();
                       return true;
                   }

               }
           }
           #endregion
           if (Program.W.IsReady() && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "BlindMonkWOne")
           {
               InventorySlot invSlot = getBestWardItem();
               Items.UseItem((int)invSlot.Id, position );
               foreach (Obj_AI_Minion ward in ObjectManager.Get<Obj_AI_Minion>().Where(ward =>
           ward.Name.ToLower().Contains("ward") && ward.Distance(Game.CursorPos) < 250))
               {
                   if (ward != null)
                   {
                       Program.W.CastOnUnit(ward);
                       Program.W.Cast();
                       return true;

                   }

               }
           }
           return false;
       }
       public static bool jump()
       {
           #region ward ya existe
           if (Program.W.IsReady())
           {
               foreach (Obj_AI_Minion ward in ObjectManager.Get<Obj_AI_Minion>().Where(ward =>
                    ward.Name.ToLower().Contains("ward") && ward.Distance(Game.CursorPos) < 250))
               {
                   if (ward != null)
                   {
                       Program.W.CastOnUnit(ward);
                       Program.W.Cast();
                       return true;

                   }
               
               }

               foreach (
                   Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.Distance(Game.CursorPos) < 250 && !hero.IsDead))
               {
                   if (hero != null)
                   {
                       Program.W.CastOnUnit(hero);
                       Program.W.Cast();
                       return true;

                   }
             
               }

               foreach (Obj_AI_Minion minion in ObjectManager.Get<Obj_AI_Minion>().Where(minion =>
                   minion.Distance(Game.CursorPos) < 250))
               {
                   if (minion != null)
                   {
                       Program.W.CastOnUnit(minion);
                       Program.W.Cast();
                       return true;
                   }

               }
           }
           #endregion
           if (Program.W.IsReady() && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "BlindMonkWOne")
           {
               if (Environment.TickCount <= LastPlaced + 3000) return false;
               Vector3 cursorPos = Game.CursorPos;
               Vector3 myPos = Player.ServerPosition;

               Vector3 delta = cursorPos - myPos;
               delta.Normalize();

               Vector3 wardPosition = myPos + delta * (600 - 5);
               InventorySlot invSlot = getBestWardItem();
               Items.UseItem((int)invSlot.Id, wardPosition);
               LastPlaced = Environment.TickCount;
               return true;
           }
           return false;
       }
       public static int getJumpWardId()
       {
           int[] wardIds = { 3340, 3350, 3205, 3207, 2049, 2045, 2044, 3361, 3154, 3362, 3160, 2043 };
           foreach (int id in wardIds)
           {
               if (Items.HasItem(id) && Items.CanUseItem(id))
                   return id;
           }
           return -1;
       }
       public static bool inDistance(Vector2 pos1, Vector2 pos2, float distance)
       {
           float dist2 = Vector2.DistanceSquared(pos1, pos2);
           return (dist2 <= distance * distance) ? true : false;
       }
       public static Vector3 Insecpos(Obj_AI_Hero ts)
       {
           return Game.CursorPos.Extend(ts.Position, Game.CursorPos.Distance(ts.Position) + 250);
       }
       public static Vector3 getward(Obj_AI_Hero target)
       {
           Obj_AI_Turret turret = ObjectManager.Get<Obj_AI_Turret>().Where(tur => tur.IsAlly && tur.Health > 0 && !tur.IsMe).OrderBy(tur => tur.Distance(Player.ServerPosition)).First();
           return target.ServerPosition + Vector3.Normalize(turret.ServerPosition - target.ServerPosition) * (-300);
       }
       public static bool putWard(Vector2 pos)
       {
           InventorySlot invSlot = getBestWardItem();
           Items.UseItem((int)invSlot.Id, pos);
           return true;
       }
       public static void moveTo(Vector2 Pos)
       {
           Player.IssueOrder(GameObjectOrder.MoveTo, Pos.To3D());
       }

       public static void InsecJump(Vector2 pos)
       {
           Vector2 posStart = pos;
           if (!Program.W.IsReady())
               return;
           bool wardIs = false;
           if (!inDistance(pos, Player.ServerPosition.To2D(), Program.W.Range + 15))
           {
               pos = Player.ServerPosition.To2D() + Vector2.Normalize(pos - Player.ServerPosition.To2D()) * 600;
           }

           if (!Program.W.IsReady() && Program.W.ChargedSpellName == "")
               return;
           foreach (Obj_AI_Base ally in ObjectManager.Get<Obj_AI_Base>().Where(ally => ally.IsAlly
               && !(ally is Obj_AI_Turret) && inDistance(pos, ally.ServerPosition.To2D(), 200)))
           {
               wardIs = true;
               moveTo(pos);
               if (inDistance(Player.ServerPosition.To2D(), ally.ServerPosition.To2D(), Program.W.Range + ally.BoundingRadius))
               {
                   Program.W.Cast(ally);

               }
               return;
           }
           Polygon pol;
           if ((pol = Program.map.getInWhichPolygon(pos)) != null)
           {
               if (inDistance(pol.getProjOnPolygon(pos), Player.ServerPosition.To2D(), Program.W.Range + 15) && !wardIs && inDistance(pol.getProjOnPolygon(pos), pos, 200))
               {
                   if (lastwardjump < Environment.TickCount)
                   {
                       putWard(pos);
                       lastwardjump = Environment.TickCount + 1000;
                   }
               }
           }
           else if (!wardIs)
           {
               if (lastwardjump < Environment.TickCount)
               {
                   putWard(pos);
                   lastwardjump = Environment.TickCount + 1000;
               }
           }

       }

    }
}