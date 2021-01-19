import os

line_count = 0
for root, directories, filenames in os.walk('./'):
    for filename in filenames:
        if filename.endswith('.cs') or filename.endswith('.xaml'): 
            line_count += len(open(os.path.join(root,filename), 'r').readlines())

print(f"You have painfully written {line_count} lines of code. You need to stop and get some help")
