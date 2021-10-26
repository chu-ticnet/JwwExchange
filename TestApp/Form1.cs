﻿using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TestApp {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, EventArgs e) {
            var f = new OpenFileDialog();
            f.Filter = "Jww Files|*.jww|All Files|*.*";
            if (f.ShowDialog() != DialogResult.OK) return;
            OpenFile(f.FileName);
            GC.Collect();

        }

        private void btnSave_Click(object sender, EventArgs e) {
            SaveFile("d:\\test0.jww");
            GC.Collect();
        }

        void SaveFile(string path) {
            using var a = new JwwHelper.JwwWriter();
            //JwwHelper.dllと同じフォルダに"template.jww"が必要です。
            //"template.jww"は適当なjwwファイルでそのファイルからjwwファイルのヘッダーをコピーします。
            //Headerをプログラムから設定してもいいのですが、項目が多いので大変です。
            a.InitHeader("template.jww");
            //図形オブジェクトを作ってAddData()で書き込む図形を追加します。
            //適当に斜めの線を追加します。
            //var s = new JwwHelper.JwwSen();
            //s.m_start_x = 100.0;
            //s.m_start_y = 100.0;
            //s.m_end_x = -100.0;
            //s.m_end_y = -100.0;
            for(int i = 0; i < 100; i++) {
                var s = new JwwHelper.JwwSen();
                s.m_start_x = 100.0 + i;
                s.m_start_y = 100.0;
                s.m_end_x = -100.0 + i;
                s.m_end_y = -100.0;
                a.AddData(s);
            }
            //Write()で書き込み。
            a.Write(path);
        }

        private void Form1_DragDrop(object sender, DragEventArgs e) {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (files.Length < 1) return;
            OpenFile(files[0]);
        }

        private void Form1_DragEnter(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                e.Effect = DragDropEffects.All;
            } else {
                e.Effect = DragDropEffects.None;
            }
        }
        void OpenFile(String path) {
            try {
                if (Path.GetExtension(path) == ".jww") {
                    //JwwReaderが読み込み用のクラス。Completedは読み込み完了時に実行される関数。
                    //"d:\\ccc\\"はファイルに同梱画像があった時に画像が保存されるフォルダ。
                    using var reader = new JwwHelper.JwwReader(Completed, "d:\\ccc\\");
                    reader.Read(path);
                    var a = reader.Header.m_jwwDataVersion;
                    var b = reader.Header.get_m_aStrLayName(0);

                } else if (Path.GetExtension(path) == ".jws") {
                    //jwsも読めますが、このプロジェクトでは確認用のコード上がりません。
                    using var a = new JwwHelper.JwsReader(Completed2, "d:\\ccc\\");
                    a.Read(path);
                }
            } catch (Exception exception) {
                textBox1.Text = "";
                MessageBox.Show(exception.Message, "Error");
            }
        }

        //dllでjwwファイル読み込み完了後に呼ばれます。これは確認用のコードです。
        void Completed(JwwHelper.JwwReader reader) {
            var sb = new StringBuilder();
            var header = reader.Header;
            sb.AppendLine("Paper:" + header.m_nZumen);
            sb.AppendLine("Layers=============================================");
            for (var j = 0; j < 16; j++) {
                sb.AppendLine("Layer group " + j + " Name:" + header.get_m_aStrGLayName(j) + " Scale:" + header.get_m_adScale(j));
                for (var i = 0; i < 16; i++) {
                    if (i % 2 == 1) {
                        sb.AppendLine("  Layer " + i + " Name:" + header.get_m_aStrLayName(j * 16 + i));
                    } else {
                        sb.Append("  Layer " + i + " Name:" + header.get_m_aStrLayName(j * 16 + i));
                    }
                }
            }
            sb.AppendLine("Blocks=============================================");
            sb.AppendLine("Size of blocks:" + reader.GetBlockSize());
            //foreach (var s in reader.Blocks) {
            //    sb.AppendLine(s.ToString());
            //}

            sb.AppendLine("Shapes=============================================");
            var dataList = reader.DataList;
            foreach (var s in dataList) {
                sb.Append(s.GetType().Name);
                sb.AppendLine(s.ToString());
            }
            textBox1.Text = sb.ToString();

        }
        //dllでjwsファイル読み込み完了後に呼ばれます。確認用のコードは未実装。
        void Completed2(JwwHelper.JwsReader reader) {
            var a = 1;
        }

    }
}
