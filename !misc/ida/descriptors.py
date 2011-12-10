
# taken from somewhere
def get_crefs_to( ea ):
	"""
		Retrieve a list of locations that branch to ea
	"""
	ret = []
	xrf = get_first_cref_to( ea )
	if xrf != BADADDR:
		ret.append( xrf )
	xrf = get_next_cref_to( ea, xrf )
	while xrf != BADADDR:
		ret.append( xrf )
		xrf = get_next_cref_to( ea, xrf )
	return ret        

print "<Descriptors>"

for ea in get_crefs_to(LocByName("CopyAndExpandDescriptors")):
	count = -1
	offset = -1

	while (count == -1 or offset == -1):
		ea = PrevHead(ea, ea-6)
		if ea == -1:
			print "failed to find prev head"
			quit()
		if GetMnem(ea) != "mov":
			continue
		if GetOpType(ea, 0) != o_reg:
			continue
		if GetOpType(ea, 1) != o_imm:
			continue

		if GetOperandValue(ea, 0) == 0: #eax
			count = GetOperandValue(ea, 1)
			continue
		if GetOperandValue(ea, 0) == 1: #ecx
			offset = GetOperandValue(ea, 1)
			continue

	print "<Group>"
	end = offset + count * (4*5)
	while offset < end:

		MakeDword(offset)
		MakeDword(offset+4)
		MakeDword(offset+8)
		MakeDword(offset+0xC)
		MakeDword(offset+0x10)

		name = GetString(Dword(offset))
		name = name[:name.index("\0")]
		print "<Descriptor Name=\"%s\" Position=\"%u\" Size=\"%u\" Type=\"%u\" Flags=\"%u\"/>" % (name, Dword(offset+4), Dword(offset+8), Dword(offset+0xC), Dword(offset+0x10))

		offset += (4*5)
		
	print "</Group>"

print "</Descriptors>"
