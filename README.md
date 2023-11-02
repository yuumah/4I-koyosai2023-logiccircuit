# ロジックパズル
## 操作説明
### コントローラ
左スティックまたは十字キーで移動  
丸ボタンで素子の切り替え  
三角、四角ボタンで回転  
Optionボタンでリセット、長押しで強制リセット  
### キーボード
WASDまたは矢印キーで移動  
Enterキーで素子の切り替え  
Rキーで回転  
ESCキーでリセット、長押しで強制リセット  
## ゲームの流れ
起動するとパズルがランダムで選ばれる  
難易度が高いと感じたときは長押しでリセット  
白い四角で囲われている素子が切り替え可能なので操作して電球に入力を伝える  
電球に一定時間以上入力されるとゲームクリア  
ゲームクリア後はキーを押すだけでリセット可能  
## 補足
ゲームの終了は実装していないのでAlt+F4で強制終了
## パズル作成方法
パズルの盤面はmaps/内のテキストファイルに記述する  
パズルは複数のブロックから構成されていて一つのブロックは"英字記号"+"数値"で表す  
ブロックの種類は  
- A AND
- O OR
- N NOT
- \# 何もなし
- I 縦向きのワイヤー
- L L字のワイヤー
- T T字のワイヤー
- \+ 十字のワイヤー
- _ 交差しているワイヤー
- \* 切り替え可能ブロック  

がある  
数値は回転を表し $n\tfrac{π}{2}\,\mathrm{rad}$で指定する  
切り替え可能ブロックは $\lfloor\tfrac{n}{4}\rfloor$番目の素子がデフォルトで指定される