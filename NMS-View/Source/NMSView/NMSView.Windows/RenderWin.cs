using SiliconStudio.Core.Mathematics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NMSView
{
    public partial class RenderWin : Form
    {
        public NMSViewGame Inst;

        public RenderWin(NMSViewGame Inst)
        {
            this.Inst = Inst;
            InitializeComponent();
        }

        private void openGeometryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var browser = new OpenFileDialog();
            browser.Filter = "NMS Geometry File (*.GEOMETRY.MBIN.PC)|*.GEOMETRY.MBIN.PC";
            browser.Title = "Open";

            if (browser.ShowDialog() != DialogResult.OK)
                return;

            Inst.ClearNMSEntities();
            Inst.LoadModel(browser.FileName);
        }

        private void removeFloorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var floor = Inst.SceneSystem.SceneInstance.Scene.Entities.Where(x => x.Name == "Ground");
            foreach (var entity in floor)
                Inst.EntityRemoveQueue.Enqueue(entity);
        }

        private void resetCameraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Inst.CamController.ResetCam();
        }

        private void rotateXYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var entities = Inst.SceneSystem.SceneInstance.Scene.Entities.Where(x => x is NMSEntity);

            foreach (var entity in entities)
            {
                entity.Transform.Rotation = Quaternion.RotationX(-MathUtil.PiOverTwo) * Quaternion.RotationY(-MathUtil.PiOverTwo);
            }
        }
    }
}
