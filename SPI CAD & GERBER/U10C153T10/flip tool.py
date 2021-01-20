path = "U10C153B01R0555C0.txt"
with open(path,'r') as f:
    a = f.readlines()
    newdoc = []
    newfile = open(path.replace(".txt", "_flipped.txt"),'w')
    for item in a:
        x = item.split("\t")
        val = - float(x[1])
        newitem = "{}\t{}\t{}\t{}\t{}".format(x[0], val, x[2], x[3],x[4])
        newdoc.append(newitem)
        newfile.write(newitem)

