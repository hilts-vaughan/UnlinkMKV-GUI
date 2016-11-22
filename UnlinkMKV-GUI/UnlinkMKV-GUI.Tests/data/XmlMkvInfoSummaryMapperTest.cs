using System;
using UnlinkMKV_GUI.data;
using UnlinkMKV_GUI.data.xml;
using Xunit;
using Xunit.Abstractions;

namespace UnlinkMKV_GUI.Tests.data
{

    public class XmlMkvInfoSummaryMapperTest
    {
        private readonly ITestOutputHelper _helper;

        public XmlMkvInfoSummaryMapperTest(ITestOutputHelper helper)
        {
            _helper = helper;
        }

        [Fact]
        public void TestStandardMethod()
        {
            XmlMkvInfoSummaryMapper mapper = new XmlMkvInfoSummaryMapper();
            const string data = @"
+ EBML head
|+ EBML version: 1
|+ EBML read version: 1
|+ EBML maximum ID length: 4
|+ EBML maximum size length: 8
|+ Doc type: matroska
|+ Doc type version: 4
|+ Doc type read version: 2
+ Segment, size 756458612
|+ Seek head (subentries will be skipped)
|+ EbmlVoid (size: 4013)
|+ Segment information
| + Timecode scale: 1000000
| + Muxing application: libebml v1.3.0 + libmatroska v1.4.0
| + Writing application: mkvmerge v6.0.0 ('Coming Up For Air') built on Jan 20 2013 09:52:00
| + Duration: 1396.360s (00:23:16.360)
| + Date: Mon Mar 18 23:40:26 2013 UTC
| + Title: Nisemonogatari 01
| + Segment UID: 0xbc 0xfe 0x9f 0x6d 0x5e 0xa3 0xa9 0x70 0x99 0x7c 0x32 0x17 0x3f 0x06 0x60 0x22
|+ Segment tracks
| + A track
|  + Track number: 1 (track ID for mkvmerge & mkvextract: 0)
|  + Track UID: 3740095644
|  + Track type: video
|  + Lacing flag: 0
|  + MinCache: 1
|  + Codec ID: V_MPEG4/ISO/AVC
|  + CodecPrivate, length 47 (h.264 profile: High 10 @L5.0)
|  + Default duration: 41.708ms (23.976 frames/fields per second for a video track)
|  + Language: und
|  + Name: Nisemonogatari 01
|  + Video track
|   + Pixel width: 1920
|   + Pixel height: 1080
|   + Display width: 1920
|   + Display height: 1080
| + A track
|  + Track number: 2 (track ID for mkvmerge & mkvextract: 1)
|  + Track UID: 2576664210
|  + Track type: audio
|  + Codec ID: A_FLAC
|  + CodecPrivate, length 154
|  + Default duration: 85.333ms (11.719 frames/fields per second for a video track)
|  + Language: jpn
|  + Name: 2.0 FLAC
|  + Audio track
|   + Sampling frequency: 48000
|   + Channels: 2
|   + Bit depth: 16
| + A track
|  + Track number: 3 (track ID for mkvmerge & mkvextract: 2)
|  + Track UID: 1317315370984360101
|  + Track type: subtitles
|  + Lacing flag: 0
|  + Codec ID: S_TEXT/ASS
|  + CodecPrivate, length 5185
|  + Name: English
|+ EbmlVoid (size: 1120)
|+ Attachments
| + Attached
|  + File name: Hultog Italic.ttf
|  + Mime type: application/x-truetype-font
|  + File data, size: 37372
|  + File UID: 2445306772
| + Attached
|  + File name: Hultog.ttf
|  + Mime type: application/x-truetype-font
|  + File data, size: 33296
|  + File UID: 2020957090
| + Attached
|  + File name: AmazGoDa.ttf
|  + Mime type: application/x-truetype-font
|  + File data, size: 43044
|  + File UID: 3630482355
| + Attached
|  + File name: AmazGoDaBold.ttf
|  + Mime type: application/x-truetype-font
|  + File data, size: 35076
|  + File UID: 4075561618
| + Attached
|  + File name: Andyb.TTF
|  + Mime type: application/x-truetype-font
|  + File data, size: 70324
|  + File UID: 1824581507
| + Attached
|  + File name: ANNA.ttf
|  + Mime type: application/x-truetype-font
|  + File data, size: 76000
|  + File UID: 642960046
| + Attached
|  + File name: Arena Outline.ttf
|  + Mime type: application/x-truetype-font
|  + File data, size: 33233
|  + File UID: 186099121
| + Attached
|  + File name: ariah_.ttf
|  + Mime type: application/x-truetype-font
|  + File data, size: 181020
|  + File UID: 912525466
| + Attached
|  + File name: arial.ttf
|  + Mime type: application/x-truetype-font
|  + File data, size: 766656
|  + File UID: 1718635149
| + Attached
|  + File name: Arialic Hollow.ttf
|  + Mime type: application/x-truetype-font
|  + File data, size: 181020
|  + File UID: 1909604002
| + Attached
|  + File name: ARLRDBD.TTF
|  + Mime type: application/x-truetype-font
|  + File data, size: 45260
|  + File UID: 64785322
| + Attached
|  + File name: AUBREY1__.TTF
|  + Mime type: application/x-truetype-font
|  + File data, size: 26872
|  + File UID: 510261437
| + Attached
|  + File name: CatShop.ttf
|  + Mime type: application/x-truetype-font
|  + File data, size: 79588
|  + File UID: 2756332423
| + Attached
|  + File name: CENTURY.TTF
|  + Mime type: application/x-truetype-font
|  + File data, size: 165248
|  + File UID: 593692811
| + Attached
|  + File name: CENTURYO.TTF
|  + Mime type: application/x-truetype-font
|  + File data, size: 38128
|  + File UID: 3101324205
| + Attached
|  + File name: Coprgtb.TTF
|  + Mime type: application/x-truetype-font
|  + File data, size: 61552
|  + File UID: 3011818924
| + Attached
|  + File name: DanteMTStd-Bold.otf
|  + Mime type: application/x-truetype-font
|  + File data, size: 54476
|  + File UID: 2857265596
| + Attached
|  + File name: Disney_Simple.ttf
|  + Mime type: application/x-truetype-font
|  + File data, size: 54908
|  + File UID: 380671634
| + Attached
|  + File name: edosz.ttf
|  + Mime type: application/x-truetype-font
|  + File data, size: 48820
|  + File UID: 1146176129
| + Attached
|  + File name: Fansub Block-Ozaki.ttf
|  + Mime type: application/x-truetype-font
|  + File data, size: 1812
|  + File UID: 4251068810
| + Attached
|  + File name: fansubBlock_0.ttf
|  + Mime type: application/x-truetype-font
|  + File data, size: 3364
|  + File UID: 3748357566
| + Attached
|  + File name: FNT_BS.TTF
|  + Mime type: application/x-truetype-font
|  + File data, size: 20988
|  + File UID: 2420582037
| + Attached
|  + File name: georgia.ttf
|  + Mime type: application/x-truetype-font
|  + File data, size: 157080
|  + File UID: 391120276
| + Attached
|  + File name: GillSansStd.otf
|  + Mime type: application/x-truetype-font
|  + File data, size: 28880
|  + File UID: 969964701
| + Attached
|  + File name: GillSansStd-Bold.otf
|  + Mime type: application/x-truetype-font
|  + File data, size: 29668
|  + File UID: 1400380071
| + Attached
|  + File name: hongkong.ttf
|  + Mime type: application/x-truetype-font
|  + File data, size: 43544
|  + File UID: 712839100
| + Attached
|  + File name: IwaOMinPro-Bd-Fate.ttf
|  + Mime type: application/x-truetype-font
|  + File data, size: 700900
|  + File UID: 1824292757
| + Attached
|  + File name: JFRocSol.TTF
|  + Mime type: application/x-truetype-font
|  + File data, size: 50284
|  + File UID: 1671813507
| + Attached
|  + File name: jsa_lovechinese.ttf
|  + Mime type: application/x-truetype-font
|  + File data, size: 34820
|  + File UID: 1847369877
| + Attached
|  + File name: KGFallForYou.ttf
|  + Mime type: application/x-truetype-font
|  + File data, size: 29724
|  + File UID: 1050574230
| + Attached
|  + File name: KIRBY-H.TTF
|  + Mime type: application/x-truetype-font
|  + File data, size: 49888
|  + File UID: 4280517808
| + Attached
|  + File name: lightmorning.ttf
|  + Mime type: application/x-truetype-font
|  + File data, size: 23516
|  + File UID: 2164373178
| + Attached
|  + File name: mangat.ttf
|  + Mime type: application/x-truetype-font
|  + File data, size: 29964
|  + File UID: 278465691
| + Attached
|  + File name: MLSGU.TTF
|  + Mime type: application/x-truetype-font
|  + File data, size: 69555
|  + File UID: 755965107
| + Attached
|  + File name: MyriadPro-Bold.otf
|  + Mime type: application/x-truetype-font
|  + File data, size: 81436
|  + File UID: 616126625
| + Attached
|  + File name: oakwood.ttf
|  + Mime type: application/x-truetype-font
|  + File data, size: 45884
|  + File UID: 2952023949
| + Attached
|  + File name: Old_Rubber_Stamp.ttf
|  + Mime type: application/x-truetype-font
|  + File data, size: 42436
|  + File UID: 2372587694
| + Attached
|  + File name: pastel crayon.ttf
|  + Mime type: application/x-truetype-font
|  + File data, size: 303164
|  + File UID: 741350831
| + Attached
|  + File name: phillysansps.otf
|  + Mime type: application/x-truetype-font
|  + File data, size: 5064
|  + File UID: 1617570210
| + Attached
|  + File name: pixelmix.ttf
|  + Mime type: application/x-truetype-font
|  + File data, size: 30232
|  + File UID: 872785811
| + Attached
|  + File name: Plane Crash.ttf
|  + Mime type: application/x-truetype-font
|  + File data, size: 286512
|  + File UID: 3273697459
| + Attached
|  + File name: SEVEMFBR.TTF
|  + Mime type: application/x-truetype-font
|  + File data, size: 23904
|  + File UID: 4132532117
|+ Chapters
| + EditionEntry
|  + EditionFlagOrdered: 1
|  + EditionFlagHidden: 0
|  + EditionFlagDefault: 1
|  + EditionUID: 2906622092
|  + ChapterAtom
|   + ChapterUID: 3143058099
|   + ChapterTimeStart: 00:00:00.000000000
|   + ChapterTimeEnd: 00:05:56.982000000
|   + ChapterFlagHidden: 0
|   + ChapterFlagEnabled: 1
|   + ChapterDisplay
|    + ChapterString: Prologue
|    + ChapterLanguage: eng
|  + ChapterAtom
|   + ChapterUID: 3143058098
|   + ChapterTimeStart: 00:00:00.000000000
|   + ChapterTimeEnd: 00:01:30.048000000
|   + ChapterFlagHidden: 0
|   + ChapterFlagEnabled: 1
|   + ChapterSegmentUID: length 16, data: 0x88 0x79 0x26 0x47 0x4c 0x7d 0x6a 0x4a 0x98 0x4d 0x93 0x8a 0xe8 0xe0 0x11 0x92
|   + ChapterDisplay
|    + ChapterString: Opening
|    + ChapterLanguage: eng
|  + ChapterAtom
|   + ChapterUID: 3143058097
|   + ChapterTimeStart: 00:05:57.019000000
|   + ChapterTimeEnd: 00:22:40.025000000
|   + ChapterFlagHidden: 0
|   + ChapterFlagEnabled: 1
|   + ChapterDisplay
|    + ChapterString: Episode
|    + ChapterLanguage: eng
|  + ChapterAtom
|   + ChapterUID: 3143058096
|   + ChapterTimeStart: 00:00:00.000000000
|   + ChapterTimeEnd: 00:01:30.048000000
|   + ChapterFlagHidden: 0
|   + ChapterFlagEnabled: 1
|   + ChapterSegmentUID: length 16, data: 0xb4 0x6d 0x8b 0x99 0xe1 0x6b 0x17 0x36 0xae 0xe4 0x12 0x1f 0x8a 0x34 0x82 0x79
|   + ChapterDisplay
|    + ChapterString: Ending
|    + ChapterLanguage: eng
|  + ChapterAtom
|   + ChapterUID: 3143058095
|   + ChapterTimeStart: 00:22:40.059000000
|   + ChapterTimeEnd: 00:23:15.060000000
|   + ChapterFlagHidden: 0
|   + ChapterFlagEnabled: 1
|   + ChapterDisplay
|    + ChapterString: Preview
|    + ChapterLanguage: eng
|+ EbmlVoid (size: 101)
|+ Cluster
";
            var document = mapper.DecodeStringIntoDocument(data);
            this._helper.WriteLine(document.ToString());

             var info = new MkvInfo(document);


            // TODO: There's some duplication here as a result of some of the attributes
            // It won't really affect what I want to do for the application but it might into the future affect
            // something else as well..
            this._helper.WriteLine("!");
        }

    }
}