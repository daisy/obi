#!/usr/bin/ruby

# Some quick tests in Ruby for NCX manipulation

class Audio

  def initialize(src, clip_begin, clip_end)
    @src = src
    @clipBegin = clip_begin
    @clipEnd = clip_end
  end
 
  def to_s
    "@audio(#{@src}:#{@clipBegin}:#{@clipEnd})"
  end

end

class NavLabel

  def initialize(text, audio)
    @text = text
    @audio = audio
  end

  attr_writer :text, :audio

  def NavLabel.text_only(text)
    NavLabel.new(text, nil)
  end

  def NavLabel.audio_only(audio)
    NavLabel.new(nil, audio)
  end

  def to_s
    if @text != nil 
      if @audio != nil
        "<\"#{@text}\", #{@audio}>"
      else
        "\"#{@text}\""
      end
    else
      @audio.to_s
    end
  end

end

class NavPoint

  def initialize(labels, content)
    @parent = nil
    @labels = labels
    @content = content
    @children = []
  end

  attr_writer :parent

  def append_child(child)
    @children.push(child)
    child.parent = self
  end

  def show
    _show("+ ", 1)
  end

  def _show(indent, num)
    print("#{indent}#{num} ", @labels.join("/"), " = #{@content}\n")
    @children.each { |navpoint| num = navpoint._show("  #{indent}", num + 1) }
    return num
  end

end

title = NavLabel.new("Title", Audio.new("title-en.mp3", "0s", "12.2s"))
titre = NavLabel.new("Titre", Audio.new("title-fr.mp3", "0s", "10.5s"))
section_1 = NavLabel.text_only("Section 1")
section_2 = NavLabel.audio_only(Audio.new("section2.mp3", "0s", "3.4s"))
subsection_21 = NavLabel.text_only("Subsection 2.1")
subsection_21.audio = Audio.new("sub21.mp3", "0s", "12.67s")
n = NavPoint.new([title, titre], "ncx.smil#title")
s1 = NavPoint.new([section_1], "ncx.smil#section_1")
n.append_child(s1)
s2 = NavPoint.new([section_2], "ncx.smil#section_2")
n.append_child(s2)
sub21 = NavPoint.new([subsection_21], "ncx.smil#subsection_2_1")
s2.append_child(sub21)
n.show
