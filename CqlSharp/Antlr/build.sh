CWD=$(dirname -- "$0")
java -jar $CWD/antlr-4.11.1-complete.jar \
     -Dlanguage=CSharp \
     -Xexact-output-dir \
     -o $CWD/gen \
     $CWD/*.g4
     
# Move grammer cache
mv $CWD/gen/*.interp $CWD
mv $CWD/gen/*.tokens $CWD