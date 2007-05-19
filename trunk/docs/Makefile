# Make markdown documentation

XSLT =		xsltproc
INDENT =	xmllint --format
STYLESHEET =	template.xslt
HTML_FILES =	events.html index.html strip_manager_shortcuts.html \
		tasklist.html

all:	$(HTML_FILES)

%.html:	%.txt
	echo "<markdown>" > $@.markdown && \
	markdown $< >> $@.markdown && \
	echo "</markdown>" >> $@.markdown && \
	$(XSLT) $(STYLESHEET) $@.markdown | $(INDENT) - > $@ && \
	$(RM) $@.markdown

.PHONY:	clean

clean:
	$(RM) *.markdown $(HTML_FILES)
