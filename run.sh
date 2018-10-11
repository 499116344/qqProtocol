basepath=$(cd `dirname $0`; pwd)
cd $basepath/QQLoginTest
echo $basepath/QQLoginTest
dotnet build -v q
dotnet run --no-build --qq=0 "--pass="
