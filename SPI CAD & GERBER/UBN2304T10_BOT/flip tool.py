path = "UBN2304T10_BOT.txt"
with open(path,'r') as f:
    a = f.readlines()
    newdoc = []
    newfile = open(path.replace(".txt", "flip.txt"),'w')
    for item in a:
        x = item.split("\t")
        val = - float(x[1])
        newitem = "{}\t{}\t{}\t{}\t{}".format(x[0], val, x[2], x[3],x[4])
        newdoc.append(newitem)
        newfile.write(newitem)

