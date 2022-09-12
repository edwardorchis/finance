#### 打包方法
##### 第一步：编译生成Finance项目release包
使用visual studio 生成后，在../Finance/release下分别生成目录client和server，可以删除两个目录中的*.pdb和*.xml。

##### 第二步：编译生成安装可执行程序
在./setup目录中分别有client和server的安装程序代码。我们进入到server目录执行编译生成FinanceServerSetup.exe:
```
$ cd ./server
$ pyinstaller .\FinanceServerSetup.spec
```
执行完成后，在./server/dist下就已经生成了FinanceServerSetup.exe。
client同理生成FinanceClientSetup.exe。

##### 第三步：把安装可执行程序拷贝到release
首先把生成的FinanceServerSetup.exe、FinanceClientSetup.exe和package.exe拷贝到../Finance/release目录。这时该目录下文件结构如下:
```
|-- .
|-- server
|-- client
|-- FinanceServerSetup.exe
|-- FinanceClientSetup.exe
|-- package.exe
```

##### 第四步：把Finance项目的release文件打包到安装可执行程序中
命令行进入到release目录，使用package分别打包server和client
```
$ cd ../Finance/release
$ .\package.exe FinanceServerSetup.exe server
$ .\package.exe FinanceClientSetup.exe client
```
这时候的FinanceServerSetup.exe和FinanceClientSetup.exe就是最终的安装包，可以直接拷贝的生产环境进行安装。